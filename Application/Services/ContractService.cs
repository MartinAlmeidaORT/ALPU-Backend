
using Application.Interfaces.Public.Services;
using Application.QuestPDF;
using DataAccess.ExternalServices;
using Domain.Common.Inputs;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Interfaces.Public.Singletons;
using Domain.Models;
using Domain.Models.Campaign;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace Application.Services;

public class ContractService(
    ICampaignService campaignService,
    IPriceTable priceTable,
    IUnitOfWork unitOfWork,
    AmazonS3Service amazonS3Service,
    IUserService userService) : IContractService
{
    private readonly ICampaignService _campaignService = campaignService;
    private readonly IPriceTable _priceTable = priceTable;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AmazonS3Service _amazonS3Service = amazonS3Service;
    private readonly IUserService _userService = userService;

    public async Task<Result<GenerateContractPayload>> CreateContractAsync(CampaignInput input)
    {
        Country? country = await _unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        if (country == null) return Result.Fail(CountryErrors.CountryNotFound(input.CountryCode));

        Result<Campaign> campaign = await _campaignService.CreateCampaign(input);
        if (campaign.IsFailed) return Result.Fail(campaign.Errors);

        Result<PriceBreakdown> price = await campaign.Value.Calculate(input, _priceTable, _unitOfWork);
        if (price.IsFailed) return Result.Fail(price.Errors);

        Contract contract = Contract.CreateContract(
            input.ClientId,
            input.BroadcasterId,
            campaign.Value,
            price.Value.Total,
            country.CountryCode
        );

        foreach (var cs in campaign.Value.Services)
        {
            _unitOfWork.Attach(cs.Service);
        }

        contract = _unitOfWork.Contracts.CreateContract(contract);
        await _unitOfWork.SaveChangesAsync();
        contract = _unitOfWork.Contracts.GetAllContracts()
            .Where(c => c.ContractId == contract.ContractId)
            .Include(c => c.Client.Agency)
            .Include(c => c.Client.Address.Country)
            .Include(c => c.Client.Address.Department)
            .Include(c => c.Broadcaster.Address.Country)
            .Include(c => c.Broadcaster.Address.Department)
            .Single();

        var document = new ContractDocument(contract);
        contract.PdfAmazonS3Key = await _amazonS3Service.SaveContractAsync(document.GeneratePdf(), contract.ContractId);
        var url = _amazonS3Service.GetDownloadUrl(contract.PdfAmazonS3Key);

        var payload = new GenerateContractPayload()
        {
            Contract = contract,
            PdfAmazonS3Url = url
        };

        await Task.WhenAll(
            _userService.AddNotificationAsync(contract.Broadcaster, "Nuevo contrato", $"Se genero un contrato con el usuario {contract.Broadcaster.FullName}. Espera que el locutor revise y apruebe el contrato."),
            _userService.AddNotificationAsync(contract.Client, "Nuevo contrato", $"Se genero un contrato con el usuario {contract.Client.FullName}. Espera que el cliente revise y apruebe el contrato.")
        );

        await _unitOfWork.SaveChangesAsync();
        return payload;
    }

    public Task<Contract> DeleteContractAsync(int id)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Contract> GetAllContracts()
    {
        return _unitOfWork.Contracts.GetAllContracts();
    }

    public IQueryable<Contract> GetAllContracts(int userId, string role)
    {
        return role switch
        {
            "Administrator" or "Supervisor" or "Accountant" => _unitOfWork.Contracts.GetAllContracts(),
            "Client" => _unitOfWork.Contracts.GetAllContracts().Where(c => c.ClientId == userId),
            "Broadcaster" => _unitOfWork.Contracts.GetAllContracts().Where(c => c.BroadcasterId == userId),
            _ => Enumerable.Empty<Contract>().AsQueryable()
        };
    }

    public Task<Contract?> GetContractByIdAsync(int id)
    {
        return _unitOfWork.Contracts.GetByIdAsync(id);
    }

    public async Task<Result> UpdateContractAsync(UpdateContractStateInput input, int userId)
    {
        Contract? contract = _unitOfWork.Contracts
            .GetAllContracts()
            .Where(c => (c.ClientId == userId || c.BroadcasterId == userId) && c.ContractId == input.ContractId)
            .SingleOrDefault();

        if (contract == null)
        {
            return Result.Fail($"El contrato con id: {input.ContractId} no existe o no tiene acceso al mismo.");
        }

        if (contract.State == input.NewState)
        {
            return Result.Fail("El contrato ya se encuentra en ese estado.");
        }

        contract.State = input.NewState;
        if (contract.State == ContractState.Canceled)
        {
            await _amazonS3Service.MoveContractToCancelledAsync(contract.PdfAmazonS3Key);
            if (contract.Client.UserId != userId)
            {
                await _userService.AddNotificationAsync(contract.Client, $"Cancelado el contrato: {contract.ContractId}", $"");
            }
            if (contract.Broadcaster.UserId != userId)
            {
                await _userService.AddNotificationAsync(contract.Broadcaster, $"Cancelado el contrato: {contract.ContractId}", $"");
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ApproveContractAsync(int userId, int contractId)
    {
        Contract? contract = _unitOfWork.Contracts.GetAllContracts()
            .Where(c => c.ContractId == contractId)
            .SingleOrDefault();

        if (contract == null)
        {
            return Result.Fail(ContractErrors.ContractNotFound(contractId));
        }

        if (!(contract.ClientId == userId || contract.BroadcasterId == userId))
        {
            return Result.Fail(ContractErrors.UnauthorizedUser(userId));
        }

        if (contract.ClientId == userId)
        {
            contract.ClientApproved = true;
            await _userService.AddNotificationAsync(contract.Broadcaster, $"Actualizacion contrato: {contract.ContractId}", $"Cliente {contract.Client.FullName} aprobo el contrato.");
        }
        else
        {
            contract.BroadcasterApproved = true;
            await _userService.AddNotificationAsync(contract.Client, $"Actualizacion contrato: {contract.ContractId}", $"Locutor {contract.Broadcaster.FullName} aprobo el contrato.");
        }

        if (contract.BroadcasterApproved && contract.ClientApproved)
        {
            await _userService.AddNotificationAsync(contract.Client, $"El contrato: {contract.ContractId} fue aprobado y esta activo", $"El contrato tiene vigencia hasta el {contract.DueDate}");
            await _userService.AddNotificationAsync(contract.Broadcaster, $"El contrato: {contract.ContractId} fue aprobado y esta activo", $"El contrato tiene vigencia hasta el {contract.DueDate}");
            contract.State = ContractState.Active;
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<ContractUrlPayload>> GetContractPdfDownloadUrl(Contract contract)
    {
        return new ContractUrlPayload(_amazonS3Service.GetDownloadUrl(contract.PdfAmazonS3Key));
    }
}

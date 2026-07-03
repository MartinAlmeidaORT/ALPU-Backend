
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
using Domain.Models.Services;
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

        Result<(Campaign Campaign, decimal TotalPrice, decimal TotalPricePostTax)> pricing = await BuildCampaignWithPricingAsync(input);
        if (pricing.IsFailed) return Result.Fail(pricing.Errors);

        Contract contract = await PersistNewContractAsync(input, pricing.Value, country.CountryCode);

        var (PdfKey, Url) = await GenerateAndUploadContractPdfAsync(contract);
        contract.PdfAmazonS3Key = PdfKey;

        await NotifyContractCreatedAsync(contract);
        await PromoteBroadcasterIfEligibleAsync(contract.Broadcaster);

        await _unitOfWork.SaveChangesAsync();

        return new GenerateContractPayload()
        {
            Contract = contract,
            PdfAmazonS3Url = Url
        };
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
            .Include(c => c.Client)
            .Include(c => c.Broadcaster)
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
            await _amazonS3Service.MoveContractToCancelledAsync(contract.ContractId);
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
            .Include(c => c.Client)
            .Include(c => c.Broadcaster)
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

    private async Task<Result<(Campaign Campaign, decimal TotalPrice, decimal TotalPricePostTax)>> BuildCampaignWithPricingAsync(CampaignInput input)
    {
        Result<Campaign> campaign = await _campaignService.CreateCampaign(input);
        if (campaign.IsFailed) return Result.Fail(campaign.Errors);

        Result<PriceBreakdown> price = await campaign.Value.Calculate(input, _priceTable, _unitOfWork);
        if (price.IsFailed) return Result.Fail(price.Errors);

        PriceAdjustment alpuCommission = await _priceTable.GetPriceAdjustmentAsync("alpu_commission");
        PriceAdjustment accountantCommission = await _priceTable.GetPriceAdjustmentAsync("accountant_commission");
        decimal totalPricePostTax = price.Value.Total - (price.Value.Total * (alpuCommission.Amount + accountantCommission.Amount));

        return (campaign.Value, price.Value.Total, totalPricePostTax);
    }

    private async Task<Contract> PersistNewContractAsync(CampaignInput input, (Campaign Campaign, decimal TotalPrice, decimal TotalPricePostTax) pricing, string countryCode)
    {
        Contract contract = Contract.CreateContract(
            input.ClientId,
            input.BroadcasterId,
            pricing.Campaign,
            pricing.TotalPrice,
            countryCode,
            pricing.TotalPricePostTax
        );

        foreach (var cs in pricing.Campaign.Services)
        {
            _unitOfWork.Attach(cs.Service);
        }

        _unitOfWork.Contracts.CreateContract(contract);
        await _unitOfWork.SaveChangesAsync();

        contract = await _unitOfWork.Contracts.GetContractWithFullDetailsAsync(contract.ContractId);
        contract.AssignSerial(contract.Broadcaster.FirstName, contract.Broadcaster.LastName);

        return contract;
    }

    private async Task<(string PdfKey, string Url)> GenerateAndUploadContractPdfAsync(Contract contract)
    {
        var document = new ContractDocument(contract);
        string pdfKey = await _amazonS3Service.SaveContractAsync(document.GeneratePdf(), contract.ContractId);
        return (pdfKey, _amazonS3Service.GetDownloadUrl(pdfKey));
    }

    private Task NotifyContractCreatedAsync(Contract contract) =>
        Task.WhenAll(
            _userService.AddNotificationAsync(contract.Broadcaster, "Nuevo contrato", $"Se genero un contrato con el cliente {contract.Client.FullName}. Espera que lo revise y apruebe el contrato."),
            _userService.AddNotificationAsync(contract.Client, "Nuevo contrato", $"Se genero un contrato con el locutor {contract.Broadcaster.FullName}. Espera que lo revise y apruebe el contrato.")
        );

    private async Task PromoteBroadcasterIfEligibleAsync(Broadcaster broadcaster)
    {
        if (broadcaster.Contracts.Where(c => (c.State == ContractState.Active || c.State == ContractState.Paid || c.State == ContractState.Completed)).Count() <= 3) return;

        _unitOfWork.Attach(broadcaster);
        await _userService.AddNotificationAsync(broadcaster, "Llegaste a 4 contratos", "Felicitaciones! Llegaste a 4 contratos. Dejaste de ser un locutor novel y ahora eres un locutor profesional.");
        broadcaster.Category = await _unitOfWork.Broadcasters.GetCategoryByIdAsync(2);
        _unitOfWork.Broadcasters.UpdateBroadcaster(broadcaster);
    }
}

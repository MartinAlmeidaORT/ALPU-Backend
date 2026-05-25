
using Application.Interfaces.Public.Services;
using Domain.Common.Inputs;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Singletons;
using Domain.Models;
using Domain.Models.Campaign;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace Application.Services;

public class ContractService(ICampaignService campaignService, IPriceTable priceTable, IUnitOfWork unitOfWork) : IContractService
{
    private readonly ICampaignService _campaignService = campaignService;
    private readonly IPriceTable _priceTable = priceTable;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Contract>> CreateContractAsync(CampaignInput input)
    {
        Country? country = await _unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        if (country == null) return Result.Fail(CountryErrors.CountryNotFound(input.CountryCode));

        Result<Campaign> campaign = await _campaignService.CreateCampaign(input);
        if (campaign.IsFailed) return Result.Fail(campaign.Errors);

        Result<PriceBreakdown> price = await campaign.Value.Calculate(input, _priceTable, _unitOfWork);
        if (price.IsFailed) return Result.Fail(price.Errors);

        Contract contract = new()
        {
            ClientId = input.ClientId,
            BroadcasterId = input.BroadcasterId,
            Date = new DateOnly(),
            DueDate = campaign.Value.GetExpireDate(),
            Campaigns = [campaign.Value],
            Country = country,
            TotalPrice = price.Value.Total
        };

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
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Contrato_Prueba.pdf");
        document.GeneratePdf(filePath);


        return contract;
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
        }
        else
        {
            contract.BroadcasterApproved = true;
        }

        if (contract.BroadcasterApproved && contract.ClientApproved)
        {
            contract.State = ContractState.Active;
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}

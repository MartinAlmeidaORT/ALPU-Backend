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
using Npgsql;
using QuestPDF.Fluent;

namespace Application.Services;

public class ContractService(
    ICampaignService campaignService,
    IPriceTable priceTable,
    IUnitOfWork unitOfWork,
    AmazonS3Service amazonS3Service,
    IUserService userService) : IContractService
{
    private const int MaxSerialAssignAttempts = 3;

    private readonly ICampaignService _campaignService = campaignService;
    private readonly IPriceTable _priceTable = priceTable;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AmazonS3Service _amazonS3Service = amazonS3Service;
    private readonly IUserService _userService = userService;

    public async Task<Result<GenerateContractPayload>> CreateContractAsync(CampaignInput input)
    {
        Contract? original = null;

        if (input.ContractId is int replacesContractId)
        {
            Result<Contract> cancelResult = await CancelContractForReplacementAsync(replacesContractId);
            if (cancelResult.IsFailed) return Result.Fail(cancelResult.Errors);
            original = cancelResult.Value;
        }

        Country? country = await _unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        if (country == null) return Result.Fail(CountryErrors.CountryNotFound(input.CountryCode));

        Result<(Campaign Campaign, decimal TotalPrice, decimal TotalPricePostTax)> pricing = await BuildCampaignWithPricingAsync(input);
        if (pricing.IsFailed) return Result.Fail(pricing.Errors);

        Result<Contract> persisted = await PersistNewContractAsync(input, pricing.Value, country.CountryCode, original);
        if (persisted.IsFailed) return Result.Fail(persisted.Errors);

        Contract contract = persisted.Value;

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
        User? user = await _unitOfWork.Users.GetUserByIdAsync(userId);

        if (user == null)
        {
            return Result.Fail(UserErrors.UserNotFound(userId));
        }

        Contract? contract = _unitOfWork.Contracts
            .GetAllContracts()
            .Where(c => c.ContractId == input.ContractId)
            .Include(c => c.Client)
            .Include(c => c.Broadcaster)
            .SingleOrDefault();

        if (contract == null)
        {
            return Result.Fail(ContractErrors.ContractNotFound(input.ContractId));
        }

        if (user is Accountant || !(contract?.ClientId == userId || contract?.BroadcasterId == userId))
        {
            return Result.Fail(ContractErrors.UnauthorizedUser(userId));
        }

        if (contract.State == input.NewState)
        {
            return Result.Fail(ContractErrors.RedundantStateUpdate());
        }

        contract.State = input.NewState;
        if (contract.State == ContractState.Canceled)
        {
            contract.PdfAmazonS3Key = await _amazonS3Service.MoveContractToCancelledAsync(contract.ContractId);
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

    public async Task<Result<ContractUrlPayload>> GetContractPdfDownloadUrl(int contractId)
    {
        Contract? contract = _unitOfWork.Contracts.GetAllContracts()
            .Where(c => c.ContractId == contractId)
            .SingleOrDefault();

        if (contract == null)
        {
            return Result.Fail(ContractErrors.ContractNotFound(contractId));
        }

        return new ContractUrlPayload(_amazonS3Service.GetDownloadUrl(contract.PdfAmazonS3Key), contract);
    }

    private async Task<Result<Contract>> CancelContractForReplacementAsync(int contractId)
    {
        Contract? original = _unitOfWork.Contracts.GetAllContracts()
            .Where(c => c.ContractId == contractId)
            .Include(c => c.Client)
            .Include(c => c.Broadcaster)
            .SingleOrDefault();

        if (original == null) return Result.Fail(ContractErrors.ContractNotFound(contractId));

        bool alreadyReplaced = await _unitOfWork.Contracts.GetAllContracts()
            .AnyAsync(c => c.ReplacesContractId == contractId);
        if (alreadyReplaced) return Result.Fail(ContractErrors.ContractAlreadyReplaced(contractId));

        original.State = ContractState.Canceled;
        await _amazonS3Service.MoveContractToCancelledAsync(original.ContractId);

        await _userService.AddNotificationAsync(original.Client, $"Contrato reemplazado: {original.ContractId}", "Se genero un nuevo contrato en su lugar.");
        await _userService.AddNotificationAsync(original.Broadcaster, $"Contrato reemplazado: {original.ContractId}", "Se genero un nuevo contrato en su lugar.");

        return Result.Ok(original);
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

    private async Task<Result<Contract>> PersistNewContractAsync(CampaignInput input, (Campaign Campaign, decimal TotalPrice, decimal TotalPricePostTax) pricing, string countryCode, Contract? original)
    {
        Contract contract = Contract.CreateContract(
            input.ClientId,
            input.BroadcasterId,
            pricing.Campaign,
            pricing.TotalPrice,
            countryCode,
            pricing.TotalPricePostTax,
            original
        );

        foreach (var cs in pricing.Campaign.Services)
        {
            _unitOfWork.Attach(cs.Service);
        }

        _unitOfWork.Contracts.CreateContract(contract);
        await _unitOfWork.SaveChangesAsync();

        contract = await _unitOfWork.Contracts.GetContractWithFullDetailsAsync(contract.ContractId);


        for (int attempt = 1; attempt <= MaxSerialAssignAttempts; attempt++)
        {
            int replacementCount = await _unitOfWork.Contracts.CountByRootIdAsync(contract.RootContractId ?? contract.ContractId);

            contract.AssignSerial(contract.Broadcaster.FirstName, contract.Broadcaster.LastName, contract.RootContractId, replacementCount);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok(contract);
            }
            catch (DbUpdateException ex) when (IsUniqueSerialViolation(ex) && attempt < MaxSerialAssignAttempts)
            {
                // Another reissue took this letter first — loop and recompute with a fresh count.
            }
        }

        return Result.Fail(ContractErrors.SerialGenerationConflict(contract.ContractId));
    }

    private static bool IsUniqueSerialViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "uq_contract_serial" };

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

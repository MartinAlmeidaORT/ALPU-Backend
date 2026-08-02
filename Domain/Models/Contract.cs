using Domain.Common;
using Domain.Common.Abstracts;
using Domain.Common.Errors;
using Domain.Enums;

namespace Domain.Models;

public class Contract : Entity
{
    internal Contract() { }

    public static Contract CreateContract(int clientId, int broadcasterId, Campaign.Campaign campaign, decimal price, string countryCode, decimal totalPricePostTax, Contract? replacesContract = null)
    {
        return new()
        {
            ClientId = clientId,
            BroadcasterId = broadcasterId,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = campaign.GetExpireDate(),
            Campaigns = [campaign],
            CountryCode = countryCode,
            TotalPrice = price,
            TotalPricePostTax = totalPricePostTax,
            ReplacesContractId = replacesContract?.ContractId,
            RootContractId = replacesContract?.RootContractId ?? replacesContract?.ContractId
        };
    }

    public int ContractId { get; set; }

    public string? ContractSerial { get; set; }

    public int? RootContractId { get; set; } = null;

    public int? ReplacesContractId { get; set; }

    public Contract? ReplacesContract { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public bool ClientApproved { get; set; }

    public int BroadcasterId { get; set; }

    public virtual Broadcaster Broadcaster { get; set; } = null!;

    public bool BroadcasterApproved { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal TotalPrice { get; set; }

    public ContractState State { get; set; }

    public int TermYears { get; set; }

    public string PdfAmazonS3Key { get; set; }

    public virtual ICollection<Campaign.Campaign> Campaigns { get; set; } = [];

    public virtual ICollection<Bill> Bills { get; set; } = [];

    public string CountryCode { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public decimal TotalPricePostTax { get; set; }

    public void AssignSerial(string broadcasterFirstName, string broadcasterLastName, int? replacedRootContractId, int replacementCount)
    {
        int RootContractId = replacedRootContractId ?? ContractId;
        ContractSerial = ContractSerialGenerator.Generate(broadcasterFirstName, broadcasterLastName, BroadcasterId, RootContractId, replacementCount);
    }
}
public static class ContractErrors
{
    public class ContractNotFoundError(string msg) : NotFoundError(msg);

    public class UnauthorizedUserError(string msg) : AuthError(msg);

    public class ContractNotActiveError(string msg) : ValidationError(msg);

    public class SerialGenerationConflictError(string msg) : ValidationError(msg);

    public class ContractAlreadyReplacedError(string msg) : ValidationError(msg);

    public static UnauthorizedUserError UnauthorizedUser(int userId) => new($"El usuario con id {userId} no tiene acceso a este contrato.");

    public static ContractNotFoundError ContractNotFound(int contractId) => new($"El contrato con id {contractId} no existe.");

    public static ContractNotActiveError ContractNotActive(int contractId) => new($"El contrato con id {contractId} no esta activo.");

    public static SerialGenerationConflictError SerialGenerationConflict(int contractId) => new($"No se pudo generar un numero de serie unico para el contrato con id {contractId} tras varios intentos.");

    public static ContractAlreadyReplacedError ContractAlreadyReplaced(int contractId) => new($"El contrato con id {contractId} ya fue reemplazado por otro contrato.");
}

using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;

namespace Domain.Models;

public class Bill : Entity
{
    internal Bill() { }

    public static Bill CreateBill(BillType type, string title, string description, DateOnly date, decimal amount, Contract contract, string proofAmazonS3Key)
    {
        return new()
        {
            Type = type,
            Title = title,
            Description = description,
            Date = date,
            Amount = amount,
            ContractId = contract.ContractId,
            Contract = contract,
            ProofAmazonS3Key = proofAmazonS3Key
        };
    }

    public int BillId { get; set; }

    public BillType Type { get; set; }

    public int? ContractId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string ProofAmazonS3Key { get; set; } = null!;

    public virtual Contract? Contract { get; set; }

    public static class ContractErrors
    {
        public class BillNotFoundError(string msg) : NotFoundError(msg);

        public static BillNotFoundError BillNotFound(int id) => new($"Factura con {id} no encontrado.");
    }
}

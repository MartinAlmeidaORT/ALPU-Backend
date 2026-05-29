using Domain.Common;
using Domain.Common.Errors;
using Domain.Enums;
using FluentResults;

namespace Domain.Models;

public class Bill : Entity
{
    internal Bill() { }

    public static Result<Bill> CreateBill(BillType type, string title, string description, DateOnly date, decimal amount, Contract? contract = null)
    {
        Bill newBill = new()
        {
            Type = type,
            Title = title,
            Description = description,
            Date = date,
            Amount = amount,
            ContractId = contract?.ContractId,
            Contract = contract,
        };

        newBill.Validate();
        return newBill;
    }

    public Result Validate()
    {
        Result errors = new();

        if (string.IsNullOrEmpty(Title))
        {
            errors.WithError(BillErrors.TitleIsRequired());
        }

        if (!string.IsNullOrEmpty(Description))
        {
            if (Description.Length < 10)
            {
                errors.WithError(BillErrors.DescriptionMinLength());
            }

            if (Description.Length > 200)
            {
                errors.WithError(BillErrors.DescriptionMaxLength());
            }
        }

        if (Amount <= 0)
        {
            errors.WithError(BillErrors.NegativeAmount());
        }


        return errors;
    }

    public int BillId { get; set; }

    public BillType Type { get; set; }

    public int? ContractId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string ProofFile { get; set; } = null!;

    public virtual Contract? Contract { get; set; }

    public static class BillErrors
    {
        public class BillNotFoundError(string msg) : NotFoundError(msg);

        public class TitleIsRequiredError(string msg) : ValidationError(msg);

        public class DescriptionMinLengthError(string msg) : ValidationError(msg);

        public class DescriptionMaxLengthError(string msg) : ValidationError(msg);

        public class NegativeAmountError(string msg) : ValidationError(msg);

        public static BillNotFoundError BillNotFound(int id) => new($"Factura con {id} no encontrado.");

        public static TitleIsRequiredError TitleIsRequired() => new("La factura require un titulo.");

        public static DescriptionMinLengthError DescriptionMinLength() => new("La descripcion debe tener al menos 10 caracteres");

        public static DescriptionMaxLengthError DescriptionMaxLength() => new("La descripcion puede tener hasta 200 caracteres");

        public static NegativeAmountError NegativeAmount() => new("El monto debe ser mayor a 0.");
    }
}

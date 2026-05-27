using Domain.Enums;

namespace Domain.Common.Inputs;

public record BillInput
{
    internal BillInput() { }

    public string Title = string.Empty;

    public string Description = string.Empty;

    public DateOnly Date;

    public decimal Amount;

    public BillType Type;

    public int ContractId;
}

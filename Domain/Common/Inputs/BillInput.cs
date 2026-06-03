using Domain.Enums;

namespace Domain.Common.Inputs;

public record BillInput
{
    internal BillInput() { }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public BillType Type { get; set; }

    public int? ContractId { get; set; }

    public string FileName { get; set; } = string.Empty;
}

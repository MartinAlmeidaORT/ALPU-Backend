namespace Domain.Common.Payloads;

public record CalculateContractPayload
{
    public decimal TotalPrice { get; init; }
    public ServicePricePayload[] ServicePrice { get; init; } = [];
}

public record ServicePricePayload
{
    public string PieceName { get; init; } = null!;
    public string Service { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public int? DurationId { get; init; }
    public int? Variants { get; init; }
    public decimal TotalPriceWithDiscount { get; init; }
    public ServiceFlagPayload[] ServiceFlags { get; init; } = null!;
}

public record ServiceFlagPayload(bool IsOn, string Label);

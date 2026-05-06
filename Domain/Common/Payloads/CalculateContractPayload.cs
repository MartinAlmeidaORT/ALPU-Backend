using Domain.Models;

namespace Domain.Common.Payloads;

public record CalculateContractPayload
{
    public decimal TotalPrice { get; init; }
    public ServicePricePayload[] ServicePrice { get; init; }
    // public (decimal, decimal)[] ServicePriceWithDiscount { get; init; }
}

public record ServicePricePayload
{
    public Service Service { get; init; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalPriceWithDiscount { get; init; }
}

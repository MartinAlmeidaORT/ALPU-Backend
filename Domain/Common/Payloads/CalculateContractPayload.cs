namespace Domain.Common.Payloads;

public record CalculateContractPayload
{
    public decimal TotalPrice { get; init; }
}

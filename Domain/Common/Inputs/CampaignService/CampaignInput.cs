namespace Domain.Common.Inputs.CampaignService;

public record CampaignInput
{
    public required int ClientId { get; init; }

    public required int BroadcasterId { get; init; }

    public required string Campaign { get; init; }

    public int? ContractId { get; init; }

    public string? ContractSerial { get; set; }

    public required string CountryCode { get; init; }

    public required CampaignServiceInput[] Services { get; init; }
}

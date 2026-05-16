using System.Text.Json;

namespace Domain.Common.Inputs.CampaignService;

public record CampaignServiceInput
{
    public int ServiceId { get; init; }
    public JsonElement Options { get; init; }
    public List<PieceInput> Pieces { get; init; } = null!;
}

public record PieceInput
{
    public string? Name { get; init; }
}

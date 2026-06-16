using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Common.Inputs.CampaignService;

public record BaseCampaignServiceOptions
{

}

public record NarrativeCampaignServiceOptions : BaseCampaignServiceOptions
{
    public int ExtraMinutes { get; set; }
    public int ExtraRoles { get; set; }
    public bool IsNonCommercial { get; set; }
    public bool HasLipSync { get; set; }
    public bool OnInternet { get; set; }
    public decimal? PriceOverride { get; set; }
}

public record IvrCampaignServiceOptions : BaseCampaignServiceOptions
{
    public string MessageText { get; set; } = null!;
    public int AdditionalMessages { get; set; }
    public int Updates { get; set; }
    public bool CanUpdate { get; set; }
    public bool IsInterior { get; set; }
    public decimal? PriceOverride { get; set; }


}

public record EventCampaignServiceOptions : BaseCampaignServiceOptions
{
    public bool ForMassBroadcast { get; set; }
    public DateOnly Date { get; set; }
}

public record PeriodCampaignServiceOptions : BaseCampaignServiceOptions
{
    [JsonRequired]
    public Interval Period { get; set; }
}

public record TvCampaignServiceOptions : PeriodCampaignServiceOptions
{
    public bool IsInterior { get; set; }
}

public record CinemaCampaignServiceOptions : PeriodCampaignServiceOptions
{
    public bool IsInterior { get; set; }
}

public record InternetCampaignServiceOptions : PeriodCampaignServiceOptions
{

}

public record RadioCampaignServiceOptions : PeriodCampaignServiceOptions
{
    public bool IsInterior { get; set; }
}

public record OtherMediaCampaignServiceOptions : PeriodCampaignServiceOptions
{

}

public record CameraCampaignServiceOptions : PeriodCampaignServiceOptions
{
    public bool ForInternalUse { get; set; }
}

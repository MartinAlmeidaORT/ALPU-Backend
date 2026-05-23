using Domain.Common.Inputs.CampaignService;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;

namespace Domain.Models.Campaign.Period;

public class OtherMediaCampaignService : PeriodCampaignService
{
    OtherMediaCampaignService()
    {
        Options = null!;
    }

    public OtherMediaCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, OtherMediaCampaignServiceOptions options)
    : base(service, pieces, priceTable, options)
    {
        Options = options;
    }

    private readonly new OtherMediaCampaignServiceOptions Options;
}

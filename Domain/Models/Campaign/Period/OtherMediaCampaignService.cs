using Domain.Common.Inputs.CampaignService;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;

namespace Domain.Models.Campaign.Period;

public class OtherMediaCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, OtherMediaCampaignServiceOptions options)
    : PeriodCampaignService(service, pieces, priceTable, options)
{
    private new OtherMediaCampaignServiceOptions Options => options;
}

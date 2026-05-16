using Domain.Common.Inputs.CampaignService;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;

namespace Domain.Models.Campaign.Period;

public class InternetCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, InternetCampaignServiceOptions options)
    : PeriodCampaignService(service, pieces, priceTable, options)
{
    private new InternetCampaignServiceOptions Options => options;
}

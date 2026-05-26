using Domain.Common.Inputs.CampaignService;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;

namespace Domain.Models.Campaign.Period;

public class InternetCampaignService : PeriodCampaignService
{
    internal protected InternetCampaignService()
    {
        Options = null!;
    }

    public InternetCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, InternetCampaignServiceOptions options)
    : base(service, pieces, priceTable, options)
    {
        Options = options;
    }

    private readonly new InternetCampaignServiceOptions Options;
}

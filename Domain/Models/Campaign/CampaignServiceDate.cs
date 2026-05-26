using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;

namespace Domain.Models.Campaign;

public class CampaignServiceDate : BaseCampaignService
{
    internal protected CampaignServiceDate()
    {

    }

    public CampaignServiceDate(BaseService service, List<Piece> pieces, IPriceTable priceTable, DateOnly date)
        : base(service, pieces, priceTable)
    {
        Date = date;
    }

    public DateOnly Date { get; set; }

    public override DateOnly GetExpireDate() => Date;
}

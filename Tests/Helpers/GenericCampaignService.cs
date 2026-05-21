using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Services;

namespace Tests.Helpers;

public class GenericCampaignService(BaseService service, List<Piece> pieces, IPriceTable priceTable) : BaseCampaignService(service, pieces, priceTable);

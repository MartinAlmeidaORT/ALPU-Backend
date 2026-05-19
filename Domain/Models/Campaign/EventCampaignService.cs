using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public class EventCampaignService(BaseService service, List<Piece> pieces, IPriceTable priceTable, EventCampaignServiceOptions options)
    : CampaignServiceDate(service, pieces, priceTable, options.Date)
{
    private new EventCampaignServiceOptions Options => options;

    public override async Task<Result<ServiceBreakdown>> Calculate()
    {
        if (Service.BasePrice == null) throw new NullReferenceException();

        ServiceBreakdown breakdown = new()
        {
            ServiceName = Service.Name,
            ServiceType = Service.Type,
            BasePrice = (decimal)Service.BasePrice,
            SubsequentPrice = Service.ExtraPrice,
            Pieces = CalculatePieces(),
        };

        decimal subtotal = CalculateSubTotal();

        if (Options.ForMassBroadcast)
        {
            await breakdown.ApplyPriceAdjustment(nameof(Options.ForMassBroadcast).ToSnakeCase(), _priceTable);
        }

        breakdown.SubTotal = subtotal;
        return breakdown;
    }

    public override decimal CalculateSubTotal()
    {
        return Service.BasePrice ?? throw new NullReferenceException();
    }
}

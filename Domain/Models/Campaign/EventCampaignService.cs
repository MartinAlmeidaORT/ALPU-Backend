using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public class EventCampaignService : CampaignServiceDate
{
    internal protected EventCampaignService() : base()
    {
        Options = null!;
    }

    public EventCampaignService(BaseService service, List<Piece> pieces, IPriceTable priceTable, EventCampaignServiceOptions options)
        : base(service, pieces, priceTable, options.Date)
    {
        Options = options;
    }

    private readonly new EventCampaignServiceOptions Options;

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
            BeforeDiscount = CalculateSubTotal()
        };

        decimal subtotal = breakdown.BeforeDiscount;

        breakdown.SubTotal = subtotal;
        
        if (Options.ForMassBroadcast)
        {
            await breakdown.ApplyPriceAdjustment(nameof(Options.ForMassBroadcast).ToSnakeCase(), _priceTable);
        }

        return breakdown;
    }

    public override decimal CalculateSubTotal()
    {
        return Service.BasePrice ?? throw new NullReferenceException();
    }
}

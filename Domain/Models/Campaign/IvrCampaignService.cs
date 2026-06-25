using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public class IvrCampaignService : CampaignServiceDate
{
    internal protected IvrCampaignService()
    {
        Options = null!;
    }

    public IvrCampaignService(IvrService service, List<Piece> pieces, IPriceTable priceTable, IvrCampaignServiceOptions options)
        : base(service, pieces, priceTable, options.Date)
    {
        Options = options;
    }

    private readonly new IvrCampaignServiceOptions Options;

    public async override Task<Result<ServiceBreakdown>> Calculate(CampaignInput campaign)
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

        if (Options.PriceOverride != null)
        {
            breakdown.BeforeDiscount = (decimal)Options.PriceOverride;
            breakdown.SubTotal = (decimal)Options.PriceOverride;
            return breakdown;
        }

        if (string.IsNullOrWhiteSpace(Options.MessageText)) return Result.Fail("Es necesario ingresar un texto en el servicio IVR.");

        breakdown.BeforeDiscount = CalculateSubTotal();
        decimal subtotal = breakdown.BeforeDiscount;

        if (Options.Updates > 0)
        {
            for (int i = 0; i < Options.Updates; i++)
            {
                await breakdown.ApplyPriceAdjustment(nameof(Options.Updates).ToSnakeCase(), _priceTable);
            }
        }

        if (Options.IsInterior)
        {
            await breakdown.ApplyPriceAdjustment(nameof(Options.IsInterior).ToSnakeCase(), _priceTable);
        }

        breakdown.SubTotal += subtotal;
        return breakdown;
    }

    public override decimal CalculateSubTotal()
    {
        if (Service.BasePrice == null || Service.ExtraPrice == null) throw new NullReferenceException();

        decimal subtotal = (decimal)Service.BasePrice;

        if (Service is IvrService ivr)
        {
            int words = CountWords(Options.MessageText);
            if (words > 0)
            {
                RangeIvr range = ivr.RangeIvr.Last(r => words >= r.MinWord && (r.MaxWord == null || words <= r.MaxWord));
                subtotal += words * range.PricePerWord;
            }
        }

        subtotal += Options.AdditionalMessages * (decimal)Service.ExtraPrice;

        return subtotal;
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;

namespace Domain.Common.Extensions;

public static class PriceBreakdownExtensions
{
    public static async Task<PriceBreakdown> ApplyPriceAdjustment(this PriceBreakdown breakdown, string key, IPriceTable priceTable)
    {
        PriceAdjustment? priceAdjustment = await priceTable.GetPriceAdjustmentAsync(key);

        decimal discount = priceAdjustment.Type switch
        {
            PriceAdjustmentType.Percentage => breakdown.Total * priceAdjustment.Amount,
            PriceAdjustmentType.Fixed => breakdown.Total - priceAdjustment.Amount,
            _ => throw new ArgumentException()
        };

        breakdown.Total -= discount;

        breakdown.Adjustments.Add(new(key, priceAdjustment.Amount, discount, priceAdjustment.Type));
        return breakdown;
    }

    public static async Task<ServiceBreakdown> ApplyPriceAdjustment(this ServiceBreakdown breakdown, string key, IPriceTable priceTable)
    {
        PriceAdjustment? priceAdjustment = await priceTable.GetPriceAdjustmentAsync(key);

        decimal discount = priceAdjustment.Type switch
        {
            PriceAdjustmentType.Percentage => breakdown.SubTotal * priceAdjustment.Amount,
            PriceAdjustmentType.Fixed => breakdown.SubTotal - priceAdjustment.Amount,
            _ => throw new ArgumentException()
        };

        breakdown.SubTotal -= discount;

        breakdown.Adjustments.Add(new(key, priceAdjustment.Amount, discount, priceAdjustment.Type));
        return breakdown;
    }

}

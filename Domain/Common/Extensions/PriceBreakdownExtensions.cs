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
        if (priceAdjustment == null) return breakdown;

        decimal adjusted = priceAdjustment.Type switch
        {
            PriceAdjustmentType.Percentage => breakdown.Total * priceAdjustment.Amount,
            PriceAdjustmentType.Fixed => breakdown.Total + priceAdjustment.Amount, // positivo = recargo, negativo = descuento
            _ => throw new ArgumentException()
        };

        decimal difference = adjusted - breakdown.Total;
        breakdown.Adjustments.Add(new(key, priceAdjustment.Amount, difference, priceAdjustment.Type));
        breakdown.Total = adjusted;
        return breakdown;
    }

    public static async Task<ServiceBreakdown> ApplyPriceAdjustment(this ServiceBreakdown breakdown, string key, IPriceTable priceTable)
    {
        PriceAdjustment? priceAdjustment = await priceTable.GetPriceAdjustmentAsync(key);
        if (priceAdjustment == null) return breakdown;

        decimal adjusted = priceAdjustment.Type switch
        {
            PriceAdjustmentType.Percentage => breakdown.SubTotal * priceAdjustment.Amount,
            PriceAdjustmentType.Fixed => breakdown.SubTotal + priceAdjustment.Amount,
            _ => throw new ArgumentException()
        };

        decimal difference = adjusted - breakdown.SubTotal;
        breakdown.Adjustments.Add(new(key, priceAdjustment.Amount, difference, priceAdjustment.Type));
        breakdown.SubTotal = adjusted;
        return breakdown;
    }

}

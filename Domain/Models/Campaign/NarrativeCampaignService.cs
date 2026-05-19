using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public class NarrativeCampaignService(NarrativeService service, List<Piece> pieces, IPriceTable priceTable, NarrativeCampaignServiceOptions options)
    : BaseCampaignService(service, pieces, priceTable)
{
    private readonly new NarrativeService Service = service;

    private readonly new NarrativeCampaignServiceOptions Options = options;

    public override async Task<Result<ServiceBreakdown>> Calculate()
    {
        ServiceBreakdown breakdown = new()
        {
            ServiceName = Service.Name,
            ServiceType = Service.Type,
            BasePrice = Service.BasePrice ?? 0,
            SubsequentPrice = Service.ExtraPrice,
            Pieces = CalculatePieces(),
        };

        if (Options.PriceOverride != null)
        {
            breakdown.SubTotal = (decimal)Options.PriceOverride;
            return breakdown;
        }

        breakdown.SubTotal = CalculateSubTotal();

        VolumeDiscount? volumeDiscount = await _priceTable.GetVolumeDiscountAsync(Service.Type, Options.ExtraMinutes);
        if (volumeDiscount != null)
        {
            ApplyVolumeDiscount(ref breakdown, volumeDiscount);
        }

        if (Options.ExtraRoles > 0)
        {
            breakdown.SubTotal += Options.ExtraRoles * Service.RolePrice;
            breakdown.Adjustments.Add(new("Roles Extras", nameof(Options.ExtraRoles).ToSnakeCase(), Service.RolePrice, Options.ExtraRoles * Service.RolePrice, Enums.PriceAdjustmentType.Fixed));
        }

        if (Options.HasLipSync)
        {
            await breakdown.ApplyPriceAdjustment(nameof(Options.HasLipSync).ToSnakeCase(), _priceTable);
        }

        if (Options.IsNonCommercial)
        {
            await breakdown.ApplyPriceAdjustment(nameof(Options.IsNonCommercial).ToSnakeCase(), _priceTable);
        }

        if (Options.OnInternet)
        {
            await breakdown.ApplyPriceAdjustment(nameof(Options.OnInternet).ToSnakeCase(), _priceTable);
        }

        return breakdown;
    }

    public override decimal CalculateSubTotal()
    {
        if (Service.BasePrice == null || Service.ExtraPrice == null) throw new NullReferenceException();

        decimal subtotal = (decimal)Service.BasePrice;

        subtotal += (decimal)Service.ExtraPrice * Options.ExtraMinutes;

        return subtotal;
    }
}

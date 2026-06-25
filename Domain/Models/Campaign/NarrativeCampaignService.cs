using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public class NarrativeCampaignService : CampaignServiceDate
{
    internal protected NarrativeCampaignService()
    {
        Service = null!;
        Options = null!;
    }

    public NarrativeCampaignService(NarrativeService service, List<Piece> pieces, IPriceTable priceTable, NarrativeCampaignServiceOptions options)
        : base(service, pieces, priceTable, options.Date)
    {
        Service = service;
        Options = options;
    }

    private readonly new NarrativeService Service;

    private readonly new NarrativeCampaignServiceOptions Options;

    public override async Task<Result<ServiceBreakdown>> Calculate(CampaignInput campaign)
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
            breakdown.BeforeDiscount = (decimal)Options.PriceOverride;
            breakdown.SubTotal = (decimal)Options.PriceOverride;
            return breakdown;
        }

        breakdown.BeforeDiscount = CalculateSubTotal();
        breakdown.SubTotal = breakdown.BeforeDiscount;

        VolumeDiscount? volumeDiscount = await _priceTable.GetVolumeDiscountAsync(Service.Type, Options.Minutes);
        if (volumeDiscount != null)
        {
            ApplyVolumeDiscount(ref breakdown, volumeDiscount);
        }

        if (Options.ExtraRoles > 0)
        {
            breakdown.SubTotal += Options.ExtraRoles * Service.RolePrice;
            breakdown.Adjustments.Add(new(nameof(Options.ExtraRoles).ToSnakeCase(), "Roles Extras", Service.RolePrice, Options.ExtraRoles * Service.RolePrice, Enums.PriceAdjustmentType.Fixed));
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

        if (Options.Minutes > 3)
        {
            Options.Minutes -= 3;
            subtotal += (decimal)Service.ExtraPrice * Options.Minutes;
        }

        return subtotal;
    }

    public override DateOnly GetExpireDate()
    {
        Console.WriteLine("GetExpireDate en Narrative no implementado.");
        return new DateOnly();
    }
}

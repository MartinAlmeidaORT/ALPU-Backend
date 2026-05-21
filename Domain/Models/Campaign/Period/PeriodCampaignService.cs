using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign.Period;

public abstract class PeriodCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, PeriodCampaignServiceOptions options)
    : BaseCampaignService(service, pieces, priceTable)
{
    protected new PeriodCampaignServiceOptions Options { get; } = options;

    public async override Task<Result<ServiceBreakdown>> Calculate()
    {
        Services.Period? interval = null;
        if (Service is PeriodService ps)
        {
            interval = ps.Periods.FirstOrDefault(p => p.Interval == Options.Period);
        }

        if (interval == null)
        {
            return Result.Fail($"El servicio {Service.Name} no ofrece ese tiempo de contrato. {Options.Period}");
        }

        Service.BasePrice = interval.BasePrice;
        Service.ExtraPrice = interval.ExtraPrice;
        Service.FirstExtraPrice = interval.FirstExtraPrice;

        ServiceBreakdown breakdown = new()
        {
            ServiceName = Service.Name,
            ServiceType = Service.Type,
            BasePrice = interval.BasePrice,
            SubsequentPrice = Service.ExtraPrice,
            Pieces = CalculatePieces(),
            BeforeDiscount = CalculateSubTotal(),
        };
        breakdown.SubTotal = breakdown.BeforeDiscount;

        VolumeDiscount? volumeDiscount = await _priceTable.GetVolumeDiscountAsync(Service.Type, Pieces.Count);
        if (volumeDiscount != null)
        {
            ApplyVolumeDiscount(ref breakdown, volumeDiscount);
        }

        return breakdown;
    }
}

using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign.Period;

public abstract class PeriodCampaignService : BaseCampaignService
{
    internal protected PeriodCampaignService()
    {
        Options = null!;
    }

    protected PeriodCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, PeriodCampaignServiceOptions options)
        : base(service, pieces, priceTable)
    {
        Options = options;
    }

    protected readonly new PeriodCampaignServiceOptions Options;

    public async override Task<Result<ServiceBreakdown>> Calculate(CampaignInput campaign)
    {
        Services.Period? interval = null;

        if (Service is PeriodService ps)
        {
            if (Service.Type == ServiceType.TvHost && campaign.Services.Any(s => s.ServiceId == 3))
            {
                PeriodService TvService = await _priceTable.GetServiceById(3) as PeriodService;
                interval = TvService?.Periods.FirstOrDefault(p => p.Interval == Options.Period);
                interval.BasePrice = (decimal)interval.ExtraPrice;
            } else
            {
                interval = ps.Periods.FirstOrDefault(p => p.Interval == Options.Period);
            }
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

    public override DateOnly GetExpireDate()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        return Options.Period switch
        {
            Interval.OneWeek => today.AddDays(7),
            Interval.OneMonth => today.AddMonths(1),
            Interval.ThreeMonths => today.AddMonths(3),
            Interval.SixMonths => today.AddMonths(6),
            Interval.OneYear => today.AddYears(1),
            _ => throw new NotImplementedException(),
        };
    }
}

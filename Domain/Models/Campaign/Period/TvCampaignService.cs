using Domain.Common.Inputs.CampaignService;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using CaseConverter;
using FluentResults;
using Domain.Common.Payloads;
using Domain.Common.Extensions;

namespace Domain.Models.Campaign.Period;

public class TvCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, TvCampaignServiceOptions options)
    : PeriodCampaignService(service, pieces, priceTable, options)
{
    private new TvCampaignServiceOptions Options => options;

    public async override Task<Result<ServiceBreakdown>> Calculate()
    {
        Result<ServiceBreakdown> result = await base.Calculate();

        if (Options.IsInterior)
        {
            await result.Value.ApplyPriceAdjustment(nameof(Options.IsInterior).ToSnakeCase(), _priceTable);
        }

        return result.Value;
    }
}

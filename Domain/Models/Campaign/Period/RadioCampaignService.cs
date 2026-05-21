using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign.Period;

public class RadioCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, RadioCampaignServiceOptions options)
    : PeriodCampaignService(service, pieces, priceTable, options)
{
    private new RadioCampaignServiceOptions Options => options;

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

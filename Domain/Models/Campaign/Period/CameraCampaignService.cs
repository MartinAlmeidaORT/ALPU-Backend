using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign.Period;

public class CameraCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, CameraCampaignServiceOptions options)
    : PeriodCampaignService(service, pieces, priceTable, options)
{
    private new CameraCampaignServiceOptions Options => options;

    public async override Task<Result<ServiceBreakdown>> Calculate()
    {
        Result<ServiceBreakdown> result = await base.Calculate();

        if (Options.ForInternalUse)
        {
            await result.Value.ApplyPriceAdjustment(nameof(Options.ForInternalUse).ToSnakeCase(), priceTable);
        }

        return result.Value;
    }
}

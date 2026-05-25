using Domain.Common.Inputs.CampaignService;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using CaseConverter;
using FluentResults;
using Domain.Common.Payloads;
using Domain.Common.Extensions;

namespace Domain.Models.Campaign.Period;

public class TvCampaignService : PeriodCampaignService
{
    protected TvCampaignService()
    {
        Options = null!;
    }

    public TvCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, TvCampaignServiceOptions options)
        : base(service, pieces, priceTable, options)
    {
        Options = options;
    }

    private readonly new TvCampaignServiceOptions Options;

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

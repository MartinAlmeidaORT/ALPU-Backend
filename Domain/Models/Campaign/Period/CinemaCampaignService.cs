using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign.Period;

public class CinemaCampaignService : PeriodCampaignService
{
    protected CinemaCampaignService()
    {
        Options = null!;
    }

    public CinemaCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, CinemaCampaignServiceOptions options)
    : base(service, pieces, priceTable, options)
    {
        Options = options;
    }

    private readonly new CinemaCampaignServiceOptions Options;

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

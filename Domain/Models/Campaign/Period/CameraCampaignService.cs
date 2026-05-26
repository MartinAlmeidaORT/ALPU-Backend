using CaseConverter;
using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign.Period;

public class CameraCampaignService : PeriodCampaignService
{
    internal protected CameraCampaignService()
    {
        Options = null!;
    }

    public CameraCampaignService(PeriodService service, List<Piece> pieces, IPriceTable priceTable, CameraCampaignServiceOptions options)
        : base(service, pieces, priceTable, options)
    {
        Options = options;
    }

    private readonly new CameraCampaignServiceOptions Options;

    public async override Task<Result<ServiceBreakdown>> Calculate()
    {
        Result<ServiceBreakdown> result = await base.Calculate();

        if (Options.ForInternalUse)
        {
            await result.Value.ApplyPriceAdjustment(nameof(Options.ForInternalUse).ToSnakeCase(), _priceTable);
        }

        return result.Value;
    }
}

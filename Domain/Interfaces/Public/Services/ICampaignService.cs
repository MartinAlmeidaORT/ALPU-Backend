using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Models.Campaign;
using FluentResults;

namespace Domain.Interfaces.Public.Services;

public interface ICampaignService
{
    Task<Result<Campaign>> CreateCampaign(CampaignInput input);

    Task<Result<PriceBreakdown>> CalculatePrice(CampaignInput input);
}

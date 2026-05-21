using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using FluentResults;

namespace Application.Interfaces.Public.Services;

public interface ICampaignService
{
    Task<Result<PriceBreakdown>> CalculatePrice(CampaignInput input);
}

using Domain.Common.Inputs.CampaignService;
using Domain.Models.Campaign;
using FluentResults;

namespace Domain.Interfaces.Private;

public interface ICampaignServiceFactory
{
    Task<Result<BaseCampaignService>> Create(CampaignServiceInput request);
}

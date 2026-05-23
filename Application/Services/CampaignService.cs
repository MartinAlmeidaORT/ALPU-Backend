using Application.Interfaces.Public.Services;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Private;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using FluentResults;

namespace Application.Services;

public class CampaignService(ICampaignServiceFactory campaignFactory, IPriceTable priceTable, IUnitOfWork unitOfWork) : ICampaignService
{
    public readonly ICampaignServiceFactory _campaignFactory = campaignFactory;
    public readonly IPriceTable _priceTable = priceTable;
    public readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Campaign>> CreateCampaign(CampaignInput input)
    {
        List<BaseCampaignService> campaignServices = [];
        Result<BaseCampaignService> service;

        foreach (CampaignServiceInput serviceInput in input.Services)
        {
            service = await _campaignFactory.Create(serviceInput);
            if (service.IsFailed) return Result.Fail(service.Errors);
            campaignServices.Add(service.Value);
        }

        return new Campaign(input.Campaign, campaignServices);
    }

    public async Task<Result<PriceBreakdown>> CalculatePrice(CampaignInput input)
    {
        Result<Campaign> campaign = await CreateCampaign(input);

        if (campaign.IsFailed) return Result.Fail(campaign.Errors);

        return await campaign.Value.Calculate(input, _priceTable, _unitOfWork);
    }
}

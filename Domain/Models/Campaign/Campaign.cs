using Domain.Common.Extensions;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using FluentResults;

namespace Domain.Models.Campaign;

public class Campaign
{
    public int CampaignId { get; set; }

    public int ContractId { get; set; }

    public Contract Contract { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<BaseCampaignService> Services { get; set; } = null!;

    internal Campaign() { }

    public Campaign(string name, List<BaseCampaignService> services)
    {
        Name = name;
        Services = services;
    }

    public async Task<Result<PriceBreakdown>> Calculate(CampaignInput input, IPriceTable priceTable, IUnitOfWork unitOfWork)
    {
        PriceBreakdown breakdown = new();
        HashSet<ServiceType> serviceTypes = [];

        foreach (BaseCampaignService campaignService in Services)
        {
            serviceTypes.Add(campaignService.Service.Type);
            Result<ServiceBreakdown> serviceBreakdown = await campaignService.Calculate();
            if (serviceBreakdown.IsFailed) return Result.Fail(serviceBreakdown.Errors);
            breakdown.Services.Add(serviceBreakdown.Value);
            breakdown.BeforeDiscount += serviceBreakdown.Value.SubTotal;
        }
        breakdown.Total = breakdown.BeforeDiscount;

        await ApplyMultiServiceDiscount(breakdown, [.. serviceTypes], priceTable);

        Broadcaster? broadcaster = await unitOfWork.Broadcasters.GetBroadcasterByIdAsync(input.BroadcasterId);
        if (broadcaster != null && broadcaster.CategoryId == 1)
        {
            await breakdown.ApplyPriceAdjustment("new_broadcaster", priceTable);
        }

        if (input.InCash)
        {
            await breakdown.ApplyPriceAdjustment("in_cash", priceTable);
        }

        return breakdown;
    }

    public async Task ApplyMultiServiceDiscount(PriceBreakdown breakdown, List<ServiceType> services, IPriceTable priceTable)
    {
        for (int i = 0; i < services.Count; i++)
        {
            for (int j = i + 1; j < services.Count; j++)
            {
                MultiServiceDiscount? discount = await priceTable
                    .GetMultiServiceDiscountAsync(services[i], services[j]);

                if (discount != null)
                {
                    decimal baseAmount;

                    if (discount.IsDiscountForServiceBOnly)
                    {
                        ServiceBreakdown? serviceB = breakdown.Services
                            .FirstOrDefault(s => s.ServiceType == discount.ServiceB);
                        baseAmount = serviceB?.SubTotal ?? 0;
                    }
                    else
                    {
                        baseAmount = breakdown.Total;
                    }

                    decimal adjusted = discount.Type switch
                    {
                        PriceAdjustmentType.Percentage => baseAmount * discount.Amount,
                        PriceAdjustmentType.Fixed => baseAmount + discount.Amount,
                        _ => 0
                    };

                    decimal difference = adjusted - baseAmount;
                    breakdown.Adjustments.Add(new(discount.Key, discount.Name, discount.Amount, difference, discount.Type));
                    breakdown.Total += difference;
                }
            }
        }
    }

    public DateOnly GetExpireDate()
    {
        DateOnly lastExpireDate = Services[0].GetExpireDate();
        foreach (BaseCampaignService service in Services)
        {
            if (service.GetExpireDate() > lastExpireDate)
            {
                lastExpireDate = service.GetExpireDate();
            }
        }
        return lastExpireDate;
    }
}

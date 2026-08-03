using Domain.Common.Inputs.CampaignService;

namespace Domain.Interfaces.Public.Services;

public interface IPriceCalculatorService
{
    Task<decimal> CalcularTotalAsync(List<CampaignServiceInput> inputs);
}

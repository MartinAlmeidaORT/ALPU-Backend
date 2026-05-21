using Domain.Common.Inputs.CampaignService;

namespace Application.Interfaces.Public.Services;

public interface IPriceCalculatorService
{
    Task<decimal> CalcularTotalAsync(List<CampaignServiceInput> inputs);
}

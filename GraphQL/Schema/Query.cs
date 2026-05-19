using Application.Interfaces.Public.Services;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Services;
using Domain.Models;
using Domain.Models.Services;
using GraphQL.Common;

namespace GraphQL.Schema;

public class Query
{
    [GraphQLDescription("Healthcheck")]
    public string Ping() => "Pong";

    [UseProjection]
    public IQueryable<User> GetUsers([Service] IUserService userService) => userService.GetAllUsers();

    [UseProjection]
    public IQueryable<Country> GetCountries([Service] ICountryService countryService) => countryService.GetAllCountries();

    [UseProjection]
    [UseFiltering]
    public IQueryable<Department> GetDepartments([Service] IDepartmentService departmentService) => departmentService.GetAllDepartments();

    [UseSorting]
    public IQueryable<BaseService> GetServices([Service] IAlpuService alpuService) => alpuService.GetAllServices();

    public async Task<PriceBreakdown> CalculateContract(CampaignInput input, [Service] ICampaignService campaignService)
    {
        FluentResults.Result<PriceBreakdown> result = await campaignService.CalculatePrice(input);
        return result.UnwrapOrThrow();
    }

    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseSorting]
    public IQueryable<Contract> GetContracts([Service] IContractService contractService) => contractService.GetAllContracts();
}

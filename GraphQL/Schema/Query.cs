using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces.Public.Services;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Services;
using Domain.Models;
using Domain.Models.Services;
using GraphQL.Common;
using HotChocolate.Authorization;

namespace GraphQL.Schema;

public class Query
{
    [GraphQLDescription("Healthcheck")]
    public string Ping() => "Pong";

    [UseProjection]
    [UseFiltering]
    public IQueryable<User> GetUsers([Service] IUserService userService) => userService.GetAllUsers();

    [UseProjection]
    [UseFiltering]
    public IQueryable<Client> GetClients([Service] IUserService userService) => userService.GetAllClients();

    [UseProjection]
    [UseFiltering]
    public IQueryable<Broadcaster> GetBroadcasters([Service] IUserService userService) => userService.GetAllBroadcasters();

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

    [Authorize]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Contract> GetContracts(
        [Service] IContractService contractService,
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User ?? throw new NullReferenceException();
        string? role = user.FindFirstValue(ClaimTypes.Role) ?? throw new NullReferenceException();
        string? userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new NullReferenceException();

        return contractService.GetAllContracts(int.Parse(userId), role);
    }
}

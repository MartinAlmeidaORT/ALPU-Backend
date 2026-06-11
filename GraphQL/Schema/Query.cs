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
using HotChocolate.Resolvers;

namespace GraphQL.Schema;

public class Query
{
    [GraphQLDescription("Healthcheck")]
    public string Ping() => "Pong";
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
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
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        string role = user.FindFirstValue(ClaimTypes.Role)!;
        string userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

        return contractService.GetAllContracts(int.Parse(userId), role);
    }

    [Authorize]
    public async Task<ContractUrlPayload> GetContractPdfDownloadUrl(
        [Service] IContractService contractService,
        [Service] IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        int contractId)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        string role = user.FindFirstValue(ClaimTypes.Role)!;
        int userId = int.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        Contract? contract = contractService.GetAllContracts(userId, role)
            .Where(c => c.ContractId == contractId && (role == "Administrator" || role == "Supervisor" || c.ClientId == userId || c.BroadcasterId == userId))
            .SingleOrDefault();

        if (contract == null)
        {
            var error = ContractErrors.UnauthorizedUser(userId);
            resolverContext.ReportError(ErrorBuilder.New()
                        .SetMessage(error.Message)
                        .SetCode(error.GetType().Name)
                        .Build());
            return new(null!);
        }

        FluentResults.Result<ContractUrlPayload> result = await contractService.GetContractPdfDownloadUrl(contract);
        return result.UnwrapOrThrow();
    }

    [Authorize]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Bill>> GetBills([Service] IBillService billService)
    {
        return billService.GetAllBills();
    }

    [Authorize]
    public async Task<BillUrlPayload> GetBillProofDownloadUrl(
        [Service] IBillService billService,
        [Service] IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        int billId)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        string role = user.FindFirstValue(ClaimTypes.Role)!;
        int userId = int.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        Bill? bill = billService.GetAllBills()
            .Where(b => b.BillId == billId &&
                (role == "Administrator" || role == "Supervisor" || role == "Accountant" || b.Contract.ClientId == userId || b.Contract.BroadcasterId == userId))
            .SingleOrDefault();

        return new()
        {
            AmazonS3Url = billService.GetBillProofDownloadUrl(bill)
        };
    }

    [Authorize(Roles = ["Administrator", "Supervisor", "Accountant"])]
    public async Task<DashboardPayload> GetDashboard([Service] IDashboardService dashboardService)
    {
        return await dashboardService.GetDashboardDataAsync();
    }
}

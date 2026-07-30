using System.Security.Claims;
using Application.Interfaces.Public.Services;
using Domain.Common;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Private;
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

    // [UseProjection]
    [UseFiltering(typeof(Types.Filters.BroadcasterFilterInputType))]
    public IQueryable<Broadcaster> GetBroadcasters([Service] IUserService userService) => userService.GetAllBroadcasters();

    [UsePaging(IncludeTotalCount = true)]
    // [UseProjection]
    [UseFiltering(typeof(Types.Filters.BroadcasterFilterInputType))]
    public IQueryable<Broadcaster> GetBroadcastersPaged([Service] IUserService userService) => userService.GetAllBroadcasters();

    [UseProjection]
    public IQueryable<Country> GetCountries([Service] ICountryService countryService) => countryService.GetAllCountries();

    [UseProjection]
    public IQueryable<Skill> GetSkills([Service] ISkillService skillService) => skillService.GetAllSkills();

    [UseProjection]
    public IQueryable<Language> GetLanguages([Service] ILanguageService languageService) => languageService.GetAllLanguages();

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
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);
        return contractService.GetAllContracts(claims.UserId, claims.Role);
    }

    [Authorize]
    public async Task<ContractUrlPayload> GetContractPdfDownloadUrl(
        [Service] IContractService contractService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService,
        IResolverContext resolverContext,
        int contractId)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        Contract? contract = contractService.GetAllContracts(claims.UserId, claims.Role)
            .Where(c => c.ContractId == contractId && (claims.Role == "Administrator" || claims.Role == "Supervisor" || claims.Role == "Accountant" || c.ClientId == claims.UserId || c.BroadcasterId == claims.UserId))
            .SingleOrDefault();

        if (contract == null)
        {
            var error = ContractErrors.UnauthorizedUser(claims.UserId);
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
        [Service] IJwtService jwtService,
        int billId)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        Bill? bill = billService.GetAllBills()
            .Where(b => b.BillId == billId &&
                (claims.Role == "Administrator" || claims.Role == "Supervisor" || claims.Role == "Accountant" || b.Contract.ClientId == claims.UserId || b.Contract.BroadcasterId == claims.UserId))
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

    [Authorize]
    public async Task<IQueryable<Notification>> GetNotifications(
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        return userService.GetUserNotifications(claims.UserId);
    }
}

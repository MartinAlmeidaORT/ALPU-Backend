using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces.Public.Services;
using Domain.Common;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Interfaces.Private;
using Domain.Interfaces.Public.Services;
using Domain.Models;
using FluentResults;
using GraphQL.Common;
using HotChocolate.Authorization;

namespace GraphQL.Schema;

public class Mutation
{
    public async Task<AuthPayload> RegisterBroadcaster([Service] IAuthService authService, RegisterBroadcasterInput input)
    {
        FluentResults.Result<AuthPayload> result = await authService.RegisterBroadcasterAsync(input);
        return result.UnwrapOrThrow();
    }

    public async Task<AuthPayload> RegisterClient([Service] IAuthService authService, RegisterClientInput input)
    {
        FluentResults.Result<AuthPayload> result = await authService.RegisterClientAsync(input);
        return result.UnwrapOrThrow();
    }

    public async Task<AuthPayload> LoginAsync([Service] IAuthService authService, UserLoginInput input)
    {
        FluentResults.Result<AuthPayload> result = await authService.LoginAsync(input);
        return result.UnwrapOrThrow();
    }

    public async Task<GoogleAuthPayload> GoogleAuthAsync(
        GoogleAuthInput input,
        [Service] IAuthService authService)
    {
        FluentResults.Result<GoogleAuthPayload> result = await authService.GoogleAuthAsync(input);
        return result.UnwrapOrThrow();
    }

    public async Task<AuthPayload> CompleteGoogleSignUpBroadcasterAsync(
        CompleteGoogleSignUpBroadcasterInput input,
        [Service] IAuthService authService)
    {
        FluentResults.Result<AuthPayload> result = await authService.CompleteGoogleSignUpBroadcasterAsync(input);
        return result.UnwrapOrThrow();
    }

    public async Task<AuthPayload> CompleteGoogleSignUpClientAsync(
        CompleteGoogleSignUpClientInput input,
        [Service] IAuthService authService)
    {
        FluentResults.Result<AuthPayload> result = await authService.CompleteGoogleSignUpClientAsync(input);
        return result.UnwrapOrThrow();
    }

    [Authorize]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<Contract>> UpdateContractState(
        UpdateContractStateInput input,
        [Service] IContractService contractService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        Result result = await contractService.UpdateContractAsync(input, claims.UserId);
        result.UnwrapOrThrow();
        return contractService.GetAllContracts().Where(c => c.ContractId == input.ContractId);
    }

    [Authorize]
    public async Task<GenerateContractPayload> GenerateContract(
        CampaignInput input,
        [Service] IContractService contractService)
    {
        FluentResults.Result<GenerateContractPayload> contract = await contractService.CreateContractAsync(input);
        return contract.UnwrapOrThrow();
    }

    [Authorize]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<Contract>> ApproveContract(
        int contractId,
        [Service] IContractService contractService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<string> result = await contractService.ApproveContractAsync(claims.UserId, contractId);
        result.UnwrapOrThrow();
        return contractService.GetAllContracts().Where(c => c.ContractId == contractId);
    }

    [Authorize(Roles = ["Administrator", "Supervisor"])]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<User>> ApproveUser(
        UpdateUserStateInput input,
        [Service] IUserService userService)
    {
        Result result = await userService.ApproveUser(input);
        result.UnwrapOrThrow();
        return userService.GetAllUsers().Where(u => u.UserId == input.UserId);
    }

    [Authorize(Roles = ["Administrator", "Supervisor", "Accountant"])]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<RegisterBillPayload> RegisterBill(BillInput input, [Service] IBillService billService)
    {
        FluentResults.Result<RegisterBillPayload> result = await billService.RegisterBillAsync(input);
        return result.UnwrapOrThrow();
    }

    [Authorize(Roles = ["Supervisor", "Accountant"])]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<Bill> DeleteBill(int billId, [Service] IBillService billService)
    {
        FluentResults.Result<Bill> result = await billService.DeleteBillAsync(billId);
        return result.UnwrapOrThrow();
    }

    [Authorize]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<Notification> DeleteNotification(
        int notificationId,
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<Notification> result = await userService.DeleteNotificationAsync(claims.UserId, notificationId);
        return result.UnwrapOrThrow();
    }

    [Authorize]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<Notification[]> ClearNotifications(
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<Notification[]> result = await userService.DeleteAllNotificationsAsync(claims.UserId);
        return result.UnwrapOrThrow();
    }

    [Authorize]
    public async Task<ProfilePictureUploadPayload> RequestProfilePictureUploadUrl(
        string fileName,
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<ProfilePictureUploadPayload> result = await userService.RequestProfilePictureUploadUrlAsync(claims.UserId, fileName);
        return result.UnwrapOrThrow();
    }

    [Authorize]
    public async Task<User> ConfirmProfilePictureUpload(
        string key,
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<User> result = await userService.ConfirmProfilePictureUploadAsync(claims.UserId, key);
        return result.UnwrapOrThrow();
    }

    // ---- Demo (voice sample) upload — broadcasters only ----

    [Authorize(Roles = ["Broadcaster"])]
    public async Task<DemoUploadPayload> RequestDemoUploadUrl(
        string fileName,
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<DemoUploadPayload> result = await userService.RequestDemoUploadUrlAsync(claims.UserId, fileName);
        return result.UnwrapOrThrow();
    }

    [Authorize(Roles = ["Broadcaster"])]
    public async Task<Demo> ConfirmDemoUpload(
        string key,
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<Demo> result = await userService.ConfirmDemoUploadAsync(claims.UserId, key);
        return result.UnwrapOrThrow();
    }

    [Authorize(Roles = ["Broadcaster"])]
    public async Task<Broadcaster> UpdateBroadcasterProfile(
        UpdateBroadcasterProfileInput input,
        [Service] IUserService userService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IJwtService jwtService)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        JwtUserClaims claims = jwtService.GetUserClaims(user);

        FluentResults.Result<Broadcaster> result = await userService.UpdateBroadcasterProfileAsync(claims.UserId, input);
        return result.UnwrapOrThrow();
    }
}

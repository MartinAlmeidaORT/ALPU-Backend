using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces.Public.Services;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
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
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        string role = user.FindFirstValue(ClaimTypes.Role)!;
        string userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

        Result result = await contractService.UpdateContractAsync(input, int.Parse(userId));
        result.UnwrapOrThrow();
        return contractService.GetAllContracts().Where(c => c.ContractId == input.ContractId);
    }

    [Authorize]
    public async Task<Contract> GenerateContract(
        CampaignInput input,
        [Service] IContractService contractService)
    {
        FluentResults.Result<Contract> contract = await contractService.CreateContractAsync(input);
        return contract.UnwrapOrThrow();
    }

    [Authorize]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<Contract>> ApproveContract(int contractId, [Service] IContractService contractService, [Service] IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        string userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

        FluentResults.Result<string> result = await contractService.ApproveContractAsync(int.Parse(userId), contractId);
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
}

using Application.Interfaces.Public.Services;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using GraphQL.Common;

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

    public async Task<CalculateContractPayload> CalculateContract(
        CalculateContractInput input,
        [Service] IContractService contractService)
    {
        FluentResults.Result<CalculateContractPayload> result = await contractService.CalculateContract(input);
        return result.UnwrapOrThrow();
    }
}

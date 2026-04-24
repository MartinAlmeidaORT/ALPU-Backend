using Application.Interfaces.Public.Services;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using GraphQL.Common;

namespace GraphQL.Schema;

public class Mutation
{
    public async Task<AuthPayload> RegisterBroadcaster([Service] IAuthService authService, RegisterBroadcasterInput input)
        => (await authService.RegisterBroadcasterAsync(input)).UnwrapOrThrow();

    public async Task<AuthPayload> RegisterClient([Service] IAuthService authService, RegisterClientInput input)
        => (await authService.RegisterClientAsync(input)).UnwrapOrThrow();

    public async Task<AuthPayload> LoginAsync([Service] IAuthService authService, UserLoginInput input)
        => (await authService.LoginAsync(input)).UnwrapOrThrow();

    public async Task<GoogleAuthPayload> GoogleAuthAsync(
        GoogleAuthInput input,
        [Service] IAuthService authService) => (await authService.GoogleAuthAsync(input)).UnwrapOrThrow();

    public async Task<AuthPayload> CompleteGoogleSignUpBroadcasterAsync(
        CompleteGoogleSignUpBroadcasterInput input,
        [Service] IAuthService authService) => (await authService.CompleteGoogleSignUpBroadcasterAsync(input)).UnwrapOrThrow();

    public async Task<AuthPayload> CompleteGoogleSignUpClientAsync(
        CompleteGoogleSignUpClientInput input,
        [Service] IAuthService authService) => (await authService.CompleteGoogleSignUpClientAsync(input)).UnwrapOrThrow();
}

using Application.DTOs.Auth;
using Application.Interfaces.Public.Services;

namespace GraphQL.Schema;

public class Mutation
{
    public async Task<AuthPayload> RegisterBroadcaster([Service] IAuthService authService, CreateBroadcasterDTO input)
        => await authService.RegisterBroadcasterAsync(input);

    public async Task<AuthPayload> RegisterClient([Service] IAuthService authService, CreateClientDTO input)
        => await authService.RegisterClientAsync(input);

    public async Task<AuthPayload> LoginGoogleAuthAsync(
        GoogleAuthInput input,
        [Service] IAuthService authService)
        => await authService.GoogleAuthAsync(input);

    public async Task<AuthPayload> RegisterClientGoogleAuthAsync(
        RegisterClientGoogleDTO input,
        [Service] IAuthService authService)
        => await authService.GoogleAuthAsync(input);

    public async Task<AuthPayload> RegisterBroadcasterGoogleAuthAsync(
        RegisterBroadcasterGoogleDTO input,
        [Service] IAuthService authService)
        => await authService.GoogleAuthAsync(input);
}

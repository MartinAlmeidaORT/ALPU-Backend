using Application.DTOs.Auth;
using Domain.Common;

namespace Application.Interfaces.Public.Services;

public interface IAuthService
{
    Task<AuthPayload> RegisterBroadcasterAsync(CreateBroadcasterDTO input);

    Task<AuthPayload> RegisterClientAsync(CreateClientDTO input);

    Task<AuthPayload> LoginAsync(LoginUserInput input);

    Task<AuthPayload> GoogleAuthAsync(GoogleAuthInput input);
}

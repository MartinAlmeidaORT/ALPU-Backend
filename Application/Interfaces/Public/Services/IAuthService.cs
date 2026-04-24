using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;

namespace Application.Interfaces.Public.Services;

public interface IAuthService
{
    Task<AuthPayload> RegisterBroadcasterAsync(RegisterBroadcasterInput input);

    Task<AuthPayload> RegisterClientAsync(RegisterClientInput input);

    Task<AuthPayload> LoginAsync(UserLoginInput input);

    Task<GoogleAuthPayload> GoogleAuthAsync(GoogleAuthInput input);

    Task<AuthPayload> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input);

    Task<AuthPayload> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input);
}

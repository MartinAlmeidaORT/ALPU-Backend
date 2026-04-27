using Application.Common;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;

namespace Application.Interfaces.Public.Services;

public interface IAuthService
{
    Task<ResultAPI<AuthPayload>> RegisterBroadcasterAsync(RegisterBroadcasterInput input);

    Task<ResultAPI<AuthPayload>> RegisterClientAsync(RegisterClientInput input);

    Task<ResultAPI<AuthPayload>> LoginAsync(UserLoginInput input);

    Task<ResultAPI<GoogleAuthPayload>> GoogleAuthAsync(GoogleAuthInput input);

    Task<ResultAPI<AuthPayload>> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input);

    Task<ResultAPI<AuthPayload>> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input);
}

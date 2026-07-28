using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using FluentResults;

namespace Application.Interfaces.Public.Services;

public interface IAuthService
{
    Task<Result<AuthPayload>> RegisterBroadcasterAsync(RegisterBroadcasterInput input);

    Task<Result<AuthPayload>> RegisterClientAsync(RegisterClientInput input);

    Task<Result<AuthPayload>> LoginAsync(UserLoginInput input);

    Task<Result<GoogleAuthPayload>> GoogleAuthAsync(GoogleAuthInput input);

    Task<Result<AuthPayload>> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input);

    Task<Result<AuthPayload>> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input);
}

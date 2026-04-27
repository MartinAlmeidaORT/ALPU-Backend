using Domain.Common;

namespace Domain.Interfaces.Private;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> ExchangeCodeAsync(string code);
}

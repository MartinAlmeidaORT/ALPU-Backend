using System.Security.Claims;
using Domain.Common;
using Domain.Models;

namespace Domain.Interfaces.Private;

public interface IJwtService
{
    string GenerateJWT(User user);
    IReadOnlyDictionary<string, string> DecodeClaims(string token);
    JwtUserClaims GetUserClaims(ClaimsPrincipal principal);
}

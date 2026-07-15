using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Common;
using Domain.Interfaces.Private;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DataAccess.Security;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;

    public IReadOnlyDictionary<string, string> DecodeClaims(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            throw new ArgumentException("Invalid JWT format.", nameof(token));

        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

        return jwtToken.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.First().Value);
    }

    public string GenerateJWT(User user)
    {
        var key = Environment.GetEnvironmentVariable("JWT_KEY") ??
            _configuration["JWT:Secret"] ??
            throw new ApplicationException("JWT key is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("first_name",                  user.FirstName),
            new Claim("last_name",                   user.LastName),
            new Claim("account_state",               user.UserState.ToString()),
            new Claim("account_role",                user.GetType().Name),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtUserClaims GetUserClaims(ClaimsPrincipal principal)
    {
        string userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("Token missing 'sub' claim.");

        return new()
        {
            UserId = int.Parse(userId),
            Email = principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty,

            FirstName = principal.FindFirstValue("first_name") ?? string.Empty,
            LastName = principal.FindFirstValue("last_name") ?? string.Empty,
            AccountState = principal.FindFirstValue("account_state") ?? string.Empty,
            Role = principal.FindFirstValue("account_role") ?? string.Empty
        };
    }
}

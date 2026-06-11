using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace GraphQL.Common;

public class AuthSocketInterceptor : DefaultSocketSessionInterceptor
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    private readonly TokenValidationParameters _validationParams;

    public AuthSocketInterceptor(IConfiguration configuration)
    {
        _validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    }

    public override async ValueTask<ConnectionStatus> OnConnectAsync(
        ISocketSession session,
        IOperationMessagePayload connectionInitMessage,
        CancellationToken ct)
    {
        var payload = connectionInitMessage.As<SocketAuthPayload>();
        if (payload?.Authorization is null)
        {
            return ConnectionStatus.Reject();
        }

        try
        {
            var token = payload.Authorization.Replace("Bearer ", "");
            var principal = _tokenHandler.ValidateToken(token, _validationParams, out _);
            session.Connection.HttpContext.User = principal;

            return ConnectionStatus.Accept();
        }
        catch
        {
            return ConnectionStatus.Reject();
        }
    }
}

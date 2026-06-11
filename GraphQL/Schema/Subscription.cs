using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Models;

namespace GraphQL.Schema;

public class Subscription
{
    [Subscribe]
    [Topic("{userId}")]  // scoped per user
    public Notification OnNotificationAdded(
        [EventMessage] Notification notification) => notification;

    public string OnNotificationAddedTopic(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal user = httpContextAccessor.HttpContext!.User;
        return user.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
    }
}

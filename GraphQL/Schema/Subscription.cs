using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Models;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQL.Schema;

public class Subscription
{
    [Subscribe(With = nameof(SubscribeToNotifications), MessageType = typeof(Notification))]
    public Notification OnNotificationAdded(
        [EventMessage] Notification notification) => notification;

    public async IAsyncEnumerable<Notification> SubscribeToNotifications(
        [Service] ITopicEventReceiver receiver,
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        string userId = httpContextAccessor.HttpContext!.User
        .FindFirstValue(ClaimTypes.NameIdentifier)!;

        ISourceStream<Notification> stream =
            await receiver.SubscribeAsync<Notification>(userId);

        await foreach (var notification in stream.ReadEventsAsync())
        {
            yield return notification;
        }
    }
}

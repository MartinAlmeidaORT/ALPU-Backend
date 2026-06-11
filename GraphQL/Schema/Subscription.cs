using Domain.Models;

namespace GraphQL.Schema;

public class Subscription
{
    [Subscribe]
    [Topic("{userId}")]  // scoped per user
    public Notification OnNotificationAdded(
        [EventMessage] Notification notification) => notification;
}

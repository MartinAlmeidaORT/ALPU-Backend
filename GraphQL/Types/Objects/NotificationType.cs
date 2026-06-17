using Domain.Models;

namespace GraphQL.Types.Objects;

public class NotificationType : ObjectType<Notification>
{
    protected override void Configure(IObjectTypeDescriptor<Notification> descriptor)
    {
        descriptor.Name("Notification");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.NotificationId);
        descriptor.Field(x => x.UserId);
        descriptor.Field(x => x.Title);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.Date);
        descriptor.Field(x => x.IsRead);
    }
}

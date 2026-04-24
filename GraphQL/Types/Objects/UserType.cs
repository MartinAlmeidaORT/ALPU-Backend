using Domain.Models;

namespace GraphQL.Types.Objects;

public class UserInterfaceType : InterfaceType<User>
{
    protected override void Configure(IInterfaceTypeDescriptor<User> descriptor)
    {
        descriptor.Name("User");
        descriptor.Field(x => x.Password).Ignore();
        descriptor.Field(x => x.GoogleId).Ignore();
    }
}

public class BroadcasterType : ObjectType<Broadcaster>
{
    protected override void Configure(IObjectTypeDescriptor<Broadcaster> descriptor)
    {
        descriptor.Name("Broadcaster");
        descriptor.Implements<UserInterfaceType>();
        descriptor.IgnoreSensitiveFields();
    }
}

public class ClientType : ObjectType<Client>
{
    protected override void Configure(IObjectTypeDescriptor<Client> descriptor)
    {
        descriptor.Name("Client");
        descriptor.Implements<UserInterfaceType>();
        descriptor.IgnoreSensitiveFields();
    }
}

public class AdministratorType : ObjectType<Administrator>
{
    protected override void Configure(IObjectTypeDescriptor<Administrator> descriptor)
    {
        descriptor.Name("Administrator");
        descriptor.Implements<UserInterfaceType>();
        descriptor.IgnoreSensitiveFields();
    }
}

public class SupervisorType : ObjectType<Supervisor>
{
    protected override void Configure(IObjectTypeDescriptor<Supervisor> descriptor)
    {
        descriptor.Name("Supervisor");
        descriptor.Implements<UserInterfaceType>();
        descriptor.IgnoreSensitiveFields();
    }
}

public class AccountantType : ObjectType<Accountant>
{
    protected override void Configure(IObjectTypeDescriptor<Accountant> descriptor)
    {
        descriptor.Name("Accountant");
        descriptor.Implements<UserInterfaceType>();
        descriptor.IgnoreSensitiveFields();
    }
}

public static class UserInterfaceTypeExtensions
{
    public static void IgnoreSensitiveFields<T>(this IObjectTypeDescriptor<T> descriptor)
        where T : User
    {
        descriptor.Field(x => x.Password).Ignore();
        descriptor.Field(x => x.GoogleId).Ignore();
    }
}

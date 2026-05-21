using Domain.Models;

namespace GraphQL.Types.Objects;

public class UserInterfaceType : InterfaceType<User>
{
    protected override void Configure(IInterfaceTypeDescriptor<User> descriptor)
    {
        descriptor.Name("User");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.UserId);
        descriptor.Field(x => x.Email);
        descriptor.Field(x => x.FirstName);
        descriptor.Field(x => x.LastName);
        descriptor.Field(x => x.RUT);
        descriptor.Field(x => x.UserState);
        descriptor.Field(x => x.Address).Type<AddressType>();
    }
}

public class BroadcasterType : ObjectType<Broadcaster>
{
    protected override void Configure(IObjectTypeDescriptor<Broadcaster> descriptor)
    {
        descriptor.Name("Broadcaster");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<UserInterfaceType>();
        descriptor.Field(x => x.Category);
    }
}

public class ClientType : ObjectType<Client>
{
    protected override void Configure(IObjectTypeDescriptor<Client> descriptor)
    {
        descriptor.Name("Client");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<UserInterfaceType>();
        descriptor.ExtendsType<UserInterfaceType>();
        descriptor.Field(x => x.Agency).Type<AgencyType>();
    }
}

public class AdministratorType : ObjectType<Administrator>
{
    protected override void Configure(IObjectTypeDescriptor<Administrator> descriptor)
    {
        descriptor.Name("Administrator");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<UserInterfaceType>();
    }
}

public class SupervisorType : ObjectType<Supervisor>
{
    protected override void Configure(IObjectTypeDescriptor<Supervisor> descriptor)
    {
        descriptor.Name("Supervisor");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<UserInterfaceType>();
    }
}

public class AccountantType : ObjectType<Accountant>
{
    protected override void Configure(IObjectTypeDescriptor<Accountant> descriptor)
    {
        descriptor.Name("Accountant");
        descriptor.BindFieldsExplicitly();
        descriptor.Implements<UserInterfaceType>();
    }
}

public static class UserTypeExtensions
{
    public static void MapCommonFields<T>(this IObjectTypeDescriptor<T> descriptor)
        where T : User
    {
        descriptor.Field(x => x.UserId);
        descriptor.Field(x => x.Email);
        descriptor.Field(x => x.FirstName);
        descriptor.Field(x => x.LastName);
        descriptor.Field(x => x.RUT);
        descriptor.Field(x => x.UserState);
        descriptor.Field(x => x.Address).Type<AddressType>();
    }
}

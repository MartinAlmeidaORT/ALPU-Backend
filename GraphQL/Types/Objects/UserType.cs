using DataAccess.ExternalServices;
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
        descriptor.Field(x => x.Photo);
        descriptor.Field(x => x.Address).Type<AddressType>();
        descriptor.Field(x => x.Gender);
        descriptor.Field(x => x.IdentityCard);
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
        descriptor.Field(x => x.Skills).ListSize(assumedSize: 10);
        descriptor.Field(x => x.Languages).ListSize(assumedSize: 10);
        descriptor.Field(x => x.Demos).ListSize(assumedSize: 10);
        descriptor.Field(x => x.PhoneNumber);
        descriptor.Field(x => x.Website);
        descriptor.Field(x => x.Description);

        // profilePictureUrl: pre-signed GET url computed on the fly from the stored S3 key (Photo).
        // Keeping .Field(x => x.Photo) as the member expression (rather than a plain .Field("profilePictureUrl"))
        // lets HotChocolate's projection middleware know it still needs to select the Photo column,
        // even though the resolver below overrides what actually gets returned.
        descriptor.Field(x => x.Photo)
            .Name("profilePictureUrl")
            .Type<StringType>()
            .Resolve(ctx =>
            {
                Broadcaster broadcaster = ctx.Parent<Broadcaster>();
                if (string.IsNullOrEmpty(broadcaster.Photo)) return null;

                var s3Service = ctx.Service<AmazonS3Service>();
                return s3Service.GetProfilePictureUrl(broadcaster.Photo);
            });
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
        descriptor.Field(x => x.Photo);
        descriptor.Field(x => x.Address).Type<AddressType>();
        descriptor.Field(x => x.Gender).Type<AddressType>();
        descriptor.Field(x => x.IdentityCard).Type<AddressType>();
    }
}

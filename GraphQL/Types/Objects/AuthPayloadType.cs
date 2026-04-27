using Domain.Common.Payloads;

namespace GraphQL.Types.Objects;

public class AuthPayloadType : ObjectType<AuthPayload>
{
    protected override void Configure(IObjectTypeDescriptor<AuthPayload> descriptor)
    {
        descriptor.Name("AuthPayload");
        descriptor.Field(x => x.Token);
        descriptor.Field(x => x.User).Type<UserInterfaceType>();
    }
}

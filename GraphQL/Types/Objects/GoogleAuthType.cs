using Domain.Common.Payloads;

namespace GraphQL.Types.Objects;

public class GoogleAuthType : ObjectType<GoogleAuthPayload>
{
    protected override void Configure(IObjectTypeDescriptor<GoogleAuthPayload> descriptor)
    {
        descriptor.Name("GoogleAuth");
    }
}

using Domain.Models;

namespace GraphQL.Types.Objects;

public class AddressType : ObjectType<Address>
{
    protected override void Configure(IObjectTypeDescriptor<Address> descriptor)
    {
        descriptor.Name("Address");
    }
}

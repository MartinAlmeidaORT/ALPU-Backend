using Domain.Models;

namespace GraphQL.Types.Objects;

public class AddressType : ObjectType<Address>
{
    protected override void Configure(IObjectTypeDescriptor<Address> descriptor)
    {
        descriptor.Name("Address");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Department);
        descriptor.Field(x => x.City);
        descriptor.Field(x => x.Street);
        descriptor.Field(x => x.Country);
        descriptor.Field(x => x.CountryCode);
    }
}

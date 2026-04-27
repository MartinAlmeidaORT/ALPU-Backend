using Domain.Models;

namespace GraphQL.Types.Objects;

public class CountryType : ObjectType<Country>
{
    protected override void Configure(IObjectTypeDescriptor<Country> descriptor)
    {
        descriptor.Name("Country");
    }
}

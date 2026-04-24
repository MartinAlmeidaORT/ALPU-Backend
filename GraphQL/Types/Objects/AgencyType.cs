using Domain.Models;

namespace GraphQL.Types.Objects;

public class AgencyType : ObjectType<Agency>
{
    protected override void Configure(IObjectTypeDescriptor<Agency> descriptor)
    {
        descriptor.Name("Agency");
    }
}

using Domain.Models;

namespace GraphQL.Types.Objects;

public class AgencyType : ObjectType<Agency>
{
    protected override void Configure(IObjectTypeDescriptor<Agency> descriptor)
    {
        descriptor.Name("Agency");
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.AgencyId);
        descriptor.Field(x => x.Name);
    }
}

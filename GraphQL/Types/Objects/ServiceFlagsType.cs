using Domain.Common.Inputs;

namespace GraphQL.Types.Objects;

public class ServiceFlagsType : ObjectType<ServiceFlagsInput>
{
    protected override void Configure(IObjectTypeDescriptor<ServiceFlagsInput> descriptor)
    {
        descriptor.Name("ServiceFlags");
    }
}

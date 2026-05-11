using Domain.Common.Inputs;

namespace GraphQL.Types.Inputs;

public class ServiceFlagsInputType : InputObjectType<ServiceFlagsInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<ServiceFlagsInput> descriptor)
    {
        descriptor.Name("ServiceFlagsInput");
    }
}

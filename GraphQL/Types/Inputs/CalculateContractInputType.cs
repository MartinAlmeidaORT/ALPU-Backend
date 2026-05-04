using Domain.Common.Inputs;

namespace GraphQL.Types.Inputs;

public class CalculateContractInputType : InputObjectType<CalculateContractInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CalculateContractInput> descriptor)
    {
        descriptor.Name("CalculateContractInput");
    }
}

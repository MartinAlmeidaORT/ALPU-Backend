using Domain.Common.Inputs;

namespace GraphQL.Types.Inputs;

public class CalculateContractServiceInputType : InputObjectType<CalculateContractServiceInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CalculateContractServiceInput> descriptor)
    {
        descriptor.Name("CalculateContractServiceInput");
    }
}

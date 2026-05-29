using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class BillInputType : InputObjectType<BillInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<BillInput> descriptor)
    {
        descriptor.Name("BillInput");
    }
}

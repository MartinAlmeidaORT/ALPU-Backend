using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class RegisterClientInputType : InputObjectType<RegisterClientInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<RegisterClientInput> descriptor)
    {
        descriptor.Name("RegisterClientInput");
    }
}

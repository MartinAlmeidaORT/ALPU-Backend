using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class GoogleAuthInputType : InputObjectType<GoogleAuthInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<GoogleAuthInput> descriptor)
    {
        descriptor.Name("GoogleAuthInput");
    }
}

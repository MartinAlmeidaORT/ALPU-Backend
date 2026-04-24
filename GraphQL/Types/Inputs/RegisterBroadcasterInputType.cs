using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class RegisterBroadcasterInputType : InputObjectType<RegisterBroadcasterInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<RegisterBroadcasterInput> descriptor)
    {
        descriptor.Name("RegisterBroadcasterInput");
    }
}

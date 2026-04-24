using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class CompleteGoogleBroadcasterSignUpInputType : InputObjectType<CompleteGoogleSignUpBroadcasterInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CompleteGoogleSignUpBroadcasterInput> descriptor)
    {
        descriptor.Name("CompleteGoogleSignUpBroadcasterInput");
    }
}

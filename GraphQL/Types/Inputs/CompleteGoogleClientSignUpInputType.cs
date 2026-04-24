using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class CompleteGoogleClientSignUpInputType : InputObjectType<CompleteGoogleSignUpClientInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CompleteGoogleSignUpClientInput> descriptor)
    {
        descriptor.Name("CompleteGoogleSignUpClientInput");
    }
}

using Domain.Common.Inputs.Auth;

namespace GraphQL.Types.Inputs;

public class UserLoginInputType : InputObjectType<UserLoginInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<UserLoginInput> descriptor)
    {
        descriptor.Name("UserLoginInput");
    }
}

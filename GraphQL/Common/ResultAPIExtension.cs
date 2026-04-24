using Application.Common;

namespace GraphQL.Common;

public static class ResultAPIExtensions
{
    public static T UnwrapOrThrow<T>(this ResultAPI<T> result)
    {
        if (result.IsSuccess)
            return result.Value!;

        throw new GraphQLException(
            ErrorBuilder.New()
                .SetMessage(result.Error!)
                .SetCode(result.ErrorCode!)
                .SetExtension("statusCode", (int)result.StatusCode)
                .Build()
        );
    }
}

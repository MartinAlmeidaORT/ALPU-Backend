using Application.Common;

namespace GraphQL.Common;

public static class ResultAPIExtensions
{
    public static T UnwrapOrThrow<T>(this ResultAPI<T> result)
    {
        if (result.IsSuccess)
            return result.Value!;

        throw new GraphQLException(
            result.Errors.Select(error =>
                ErrorBuilder.New()
                    .SetMessage(error.Message)
                    .SetCode(result.ErrorCode)
                    .Build()
            ).ToList()
        );
    }
}

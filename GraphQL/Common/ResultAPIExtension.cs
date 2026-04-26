using Application.Common;
using Domain.Common.Errors;

namespace GraphQL.Common;

public static class ResultAPIExtensions
{
    public static T UnwrapOrThrow<T>(this ResultAPI<T> result)
    {
        if (result.IsSuccess) return result.Value!;

        IEnumerable<AppError> fatal = result.Errors.Where(error => error.Kind == ErrorKind.Internal);

        if (fatal.Any()) throw new GraphQLException(
            fatal.Select(error =>
                ErrorBuilder.New()
                    .SetMessage("An unexpected error occurred")
                    .SetCode("INTERNAL_SERVER_ERROR")
                    .Build()
            )
        );

        throw new GraphQLException(
            result.Errors.Select(error =>
                ErrorBuilder.New()
                    .SetMessage(error.Message)
                    .SetCode(result.ErrorCode)
                    .Build()
            )
        );
    }
}

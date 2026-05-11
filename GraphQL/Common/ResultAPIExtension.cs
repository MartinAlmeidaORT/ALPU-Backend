namespace GraphQL.Common;

public static class ResultAPIExtensions
{
    public static T UnwrapOrThrow<T>(this FluentResults.Result<T> result)
    {
        if (result.IsSuccess)
            return result.Value;

        // Map FluentResults errors to Hot Chocolate GraphQLException
        throw new GraphQLException(
            result.Errors.Select(error =>
                      ErrorBuilder.New()
                          .SetMessage(error.Message)
                          .SetCode(error.GetType().Name)
                          .ClearLocations()
                          .Build()
                  ).ToList()
        );
    }
}

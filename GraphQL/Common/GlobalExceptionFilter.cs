namespace GraphQL.Common;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IErrorFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger = logger;

    public IError OnError(IError error)
    {
        if (error.Code != null && error.Exception == null)
        {
            return error;
        }

        // Si ya es un GraphQLException que nosotros lanzamos, la dejamos pasar
        if (error.Exception is GraphQLException)
        {
            return error;
        }

        _logger.LogError(error.Exception, $"Unhandled exception: {error.Message}");

        return ErrorBuilder.New()
            .SetMessage("Fallo en el servidor.")
            .SetCode("INTERNAL_SERVER_ERROR")
            .SetExtension("message", "Fallo en el servidor.")
            .Build();
    }
}

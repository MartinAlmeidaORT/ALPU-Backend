namespace GraphQL.Common;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IErrorFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger = logger;

    public IError OnError(IError error)
    {
        // Si ya es un GraphQLException que nosotros lanzamos, la dejamos pasar
        if (error.Exception is null) return error;

        // Cualquier excepción no controlada → log + mensaje genérico
        _logger.LogError(error.Exception, "Unhandled exception: {Message}", error.Exception.Message);

        return ErrorBuilder.New()
            .SetMessage("An unexpected error occurred")
            .SetExtension("code", "INTERNAL_SERVER_ERROR")
            .SetException(error.Exception)
            .Build();
    }
}

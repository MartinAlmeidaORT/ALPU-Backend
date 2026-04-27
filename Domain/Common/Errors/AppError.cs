namespace Domain.Common.Errors;

public record AppError
{
    public string Message { get; }
    public ErrorKind Kind { get; }

    private AppError(string Message, ErrorKind Kind)
    {
        this.Message = Message;
        this.Kind = Kind;
    }

    public override string ToString() => $"{Kind}: {Message}";

    // FACTORIES
    public static AppError Validation(string message) => new(message, ErrorKind.Validation);
    public static AppError NotFound(string message) => new(message, ErrorKind.NotFound);
    public static AppError Unauthorized(string message) => new(message, ErrorKind.Unauthorized);
    public static AppError Forbidden(string message) => new(message, ErrorKind.Forbidden);
    public static AppError Conflict(string message) => new(message, ErrorKind.Conflict);
    public static AppError Internal(string message) => new(message, ErrorKind.Internal);
}

public enum ErrorKind
{
    Validation,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Internal
}

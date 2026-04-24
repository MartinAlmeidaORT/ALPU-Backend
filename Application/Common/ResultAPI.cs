using System.Net;

namespace Application.Common;

public class ResultAPI<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }

    private ResultAPI(T value)
    {
        IsSuccess = true;
        Value = value;
        StatusCode = HttpStatusCode.OK;
    }

    private ResultAPI(string error, string errorCode, HttpStatusCode statusCode)
    {
        IsSuccess = false;
        Error = error;
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    // FACTORIES
    public static ResultAPI<T> Success(T value) => new(value);

    public static ResultAPI<T> NotFound(string error) =>
        new(error, "NOT_FOUND", HttpStatusCode.NotFound);

    public static ResultAPI<T> Unauthorized(string error) =>
        new(error, "UNAUTHORIZED", HttpStatusCode.Unauthorized);

    public static ResultAPI<T> Forbidden(string error) =>
        new(error, "FORBIDDEN", HttpStatusCode.Forbidden);

    public static ResultAPI<T> Conflict(string error) =>
        new(error, "CONFLICT", HttpStatusCode.Conflict);

    public static ResultAPI<T> Invalid(string error) =>
        new(error, "INVALID_INPUT", HttpStatusCode.BadRequest);
}

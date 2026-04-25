using System.Net;
using Domain.Common;
using Domain.Common.Errors;

namespace Application.Common;

public class ResultAPI<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get => !IsSuccess; }
    public T? Value { get; }
    public IReadOnlyList<AppError> Errors { get; }
    public AppError? Error => Errors.FirstOrDefault();
    public string? ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }

    private ResultAPI(T value)
    {
        IsSuccess = true;
        Value = value;
        Errors = [];
        StatusCode = HttpStatusCode.OK;
    }

    private ResultAPI(string errorCode, HttpStatusCode statusCode, params IReadOnlyList<AppError> errors)
    {
        IsSuccess = false;
        Errors = errors;
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    // FACTORIES
    public static ResultAPI<T> Success(T value) => new(value);

    public static ResultAPI<T> From(Result<T, AppError> result)
    {
        if (result.IsSuccess) return new ResultAPI<T>(result.Value!);

        return result.Error?.Kind switch
        {
            ErrorKind.Validation => new("BAD_REQUEST", HttpStatusCode.BadRequest, result.Errors),
            ErrorKind.NotFound => new("NOT_FOUND", HttpStatusCode.NotFound, result.Errors),
            ErrorKind.Unauthorized => new("UNAUTHORIZED", HttpStatusCode.Unauthorized, result.Errors),
            ErrorKind.Forbidden => new("FORBIDDEN", HttpStatusCode.Forbidden, result.Errors),
            ErrorKind.Conflict => new("CONFLICT", HttpStatusCode.Conflict, result.Errors),
            _ => new("INTERNAL_SERVER_ERROR", HttpStatusCode.InternalServerError, result.Errors),
        };
    }

    public static ResultAPI<T> BadRequest(Result<AppError> result) => From(Result<T, AppError>.Failure(result.Errors));

    public static ResultAPI<T> BadRequest(string message) => From(Result<T, AppError>.Failure(AppError.Validation(message)));

    public static ResultAPI<T> NotFound(string message) => From(Result<T, AppError>.Failure(AppError.NotFound(message)));

    public static ResultAPI<T> Unauthorized(string message) => From(Result<T, AppError>.Failure(AppError.Unauthorized(message)));

    public static ResultAPI<T> Forbidden(string message) => From(Result<T, AppError>.Failure(AppError.Forbidden(message)));

    public static ResultAPI<T> Conflict(string message) => From(Result<T, AppError>.Failure(AppError.Conflict(message)));

    public static ResultAPI<T> Internal(string message) => From(Result<T, AppError>.Failure(AppError.Internal(message)));
}

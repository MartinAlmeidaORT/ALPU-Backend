using Domain.Common.Errors;

namespace Domain.Common;

public record Result<E>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get => !IsSuccess; }
    public E? Error { get => Errors.FirstOrDefault(); }
    public IReadOnlyList<E> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<E> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result<E> Success() => new(true, []);
    public static Result<E> Failure(E error) => new(false, [error]);
    public static Result<E> Failure(IReadOnlyList<E> errors) => new(false, errors);

    public static Result<E> Combine(params Result<E>[] results)
    {
        var errors = results
            .Where(r => r.IsFailure)
            .SelectMany(r => r.Errors)
            .ToList();

        return errors.Count > 0 ? Failure(errors) : Success();
    }

    public override string ToString() =>
        IsSuccess ? "Success" : $"Failure({string.Join(", ", Errors)})";
}

public record Result<T, E> : Result<E>
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, IReadOnlyList<E> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T, E> Success(T value) => new(true, value, []);
    public static new Result<T, E> Failure(E error) => new(false, default, [error]);
    public static new Result<T, E> Failure(IReadOnlyList<E> errors) => new(false, default, errors);

    public static Result<T, E> From(Result<E> result, T value) =>
        result.IsSuccess
            ? Success(value)
            : Failure(result.Errors);

    public Result<TNew, E> Map<TNew>(Func<T, TNew> mapper) =>
        IsSuccess
            ? Result<TNew, E>.Success(mapper(Value!))
            : Result<TNew, E>.Failure(Errors);

    public static Result<T, AppError> Validation(string message) => Result<T, AppError>.Failure(AppError.Validation(message));
    public static Result<T, AppError> NotFound(string message) => Result<T, AppError>.Failure(AppError.NotFound(message));
    public static Result<T, AppError> Unauthorized(string message) => Result<T, AppError>.Failure(AppError.Unauthorized(message));
    public static Result<T, AppError> Forbidden(string message) => Result<T, AppError>.Failure(AppError.Forbidden(message));
    public static Result<T, AppError> Conflict(string message) => Result<T, AppError>.Failure(AppError.Conflict(message));
    public static Result<T, AppError> Internal(string message) => Result<T, AppError>.Failure(AppError.Internal(message));

    public override string ToString() => IsSuccess ? $"Success({Value})" : $"Failure({string.Join(", ", Errors)})";
}

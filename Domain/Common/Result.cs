namespace Domain.Common;

public class Result<T, E>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public E? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(E error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T, E> Ok(T value) => new(value);
    public static Result<T, E> Err(E error) => new(error);
}

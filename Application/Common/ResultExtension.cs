using Domain.Common;
using Domain.Common.Errors;

namespace Application.Common;

public static class ResultExtension
{
    public static ResultAPI<T> ToResultAPI<T>(this Result<T, AppError> result) => ResultAPI<T>.From(result);
}

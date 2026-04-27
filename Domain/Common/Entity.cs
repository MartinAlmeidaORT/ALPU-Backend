using Domain.Common.Errors;

namespace Domain.Common;

public abstract class Entity
{
    protected static Result<AppError> Require(bool condition, string message)
    {
        return condition
            ? Result<AppError>.Success()
            : Result<AppError>.Failure(AppError.Validation(message));
    }
}

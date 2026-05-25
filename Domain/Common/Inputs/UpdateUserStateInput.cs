using Domain.Enums;

namespace Domain.Common.Inputs;

public record UpdateUserStateInput()
{
    public int UserId { get; init; }

    public UserState NewState { get; init; }
}

namespace Domain.Common.Inputs.Auth;

public record GoogleAuthInput
{
    public required string Code { get; init; }
}

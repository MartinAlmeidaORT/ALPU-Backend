namespace Domain.Common.Inputs.Auth;

public record UserLoginInput
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

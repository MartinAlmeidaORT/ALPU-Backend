using Application.DTOs.Users;

namespace Application.DTOs.Auth;

public class LoginUserInput
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

// public record AuthPayload(string? Token, ResultUserDTO? User, string? Error)
// {
//     public string? Token { get; set; } = Token;
//     public ResultUserDTO? User { get; set; } = User;
//     public string? Error { get; set; } = Error;
//     public bool Success => Error is null;

//     public static AuthPayload Fail(string error) => new(null, null, error);
// }

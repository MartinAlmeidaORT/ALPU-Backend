namespace Domain.Common;

public record JwtUserClaims
{
    public required int UserId { get; init; }
    public required string Role { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string AccountState { get; init; }
}

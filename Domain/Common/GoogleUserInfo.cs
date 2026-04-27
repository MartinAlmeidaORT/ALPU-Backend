namespace Domain.Common;

public record GoogleUserInfo
{
    public required string Subject { get; init; }
    public required string Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
}

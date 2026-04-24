namespace Domain.Common.Payloads;

public class GoogleAuthPayload
{
    public string? Token { get; init; }
    public bool RequiresRegistration { get; init; }
    public string? Subject { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

namespace Application.DTOs.Auth;

public record GoogleAuthInput
{
    public required string Token { get; set; }
}

public abstract record RegisterUserGoogleDTO : GoogleAuthInput
{
    public required string RUT { get; init; }
    public required string CountryCode { get; init; }
    public required string State { get; init; }
    public required string City { get; init; }
    public string? Street { get; init; }
}

public record RegisterBroadcasterGoogleDTO : RegisterUserGoogleDTO;

public record RegisterClientGoogleDTO : RegisterUserGoogleDTO
{
    public required string AgencyName { get; init; }
}

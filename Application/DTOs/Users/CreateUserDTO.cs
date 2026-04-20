namespace Application.DTOs.Users;

public record CreateUserDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string RUT { get; init; }
    public required string CountryCode { get; init; }
    public required string State { get; init; }
    public required string City { get; init; }
    public string? Street { get; init; }
}

public record CreateClientDTO : CreateUserDTO
{
    public required string AgencyName { get; init; }
}

public record CreateBroadcasterDTO : CreateUserDTO
{

}

using Domain.Enums;

namespace Application.DTOs.Users;

public record UpdateUserDTO
{
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? Name { get; init; }
    public string? LastName { get; init; }
    public string? RUT { get; init; }
    public string? CountryCode { get; init; }
    public string? State { get; init; }
    public string? City { get; init; }
    public string? Street { get; init; }
    public UserState? UserState { get; init; }
}

public record UpdateClientDTO : UpdateUserDTO
{
    public string? AgencyName { get; init; }
}

public record UpdateBroadcasterDTO : UpdateUserDTO
{

}

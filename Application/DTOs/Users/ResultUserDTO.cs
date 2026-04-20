using Application.DTOs.Address;
using Domain.Enums;

namespace Application.DTOs.Users;

public interface IResultUserDTO
{
    int UserId { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    ResultAddressDTO Address { get; }
    UserState UserState { get; }
}

public record ResultUserDTO : IResultUserDTO
{
    public required int UserId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required ResultAddressDTO Address { get; init; }
    public UserState UserState { get; init; }
}

public record ResultClientDTO : ResultUserDTO
{
    public required string AgencyName { get; init; }
}

public record ResultBroadcasterDTO : ResultUserDTO
{
    public required string BroadcasterCategory { get; init; }
}

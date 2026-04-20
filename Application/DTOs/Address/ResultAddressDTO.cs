using Application.DTOs.Country;

namespace Application.DTOs.Address;

public record ResultAddressDTO
{
    public required ResultCountryDTO Country { get; init; }
    public string? State { get; init; }
    public string? City { get; init; }
    public string? Street { get; init; }
}

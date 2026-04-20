namespace Application.DTOs.Country;

public record ResultCountryDTO
{
    public required string CountryCode { get; init; }
    public required string Name { get; init; }
}

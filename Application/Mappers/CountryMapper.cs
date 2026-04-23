using Riok.Mapperly.Abstractions;
using Application.DTOs.Country;
using Domain.Models;
using System.Linq.Expressions;

namespace Application.Mappers;

[Mapper]
public static partial class CountryMapper
{
    [MapperIgnoreSource(nameof(Country.RegionId))]
    [MapperIgnoreSource(nameof(Country.Region))]
    [MapperIgnoreSource(nameof(Country.Contracts))]
    public static partial ResultCountryDTO ToDTO(Country country);

    public static Expression<Func<Country, ResultCountryDTO>> ToDTOExpression() =>
        country => new ResultCountryDTO
        {
            CountryCode = country.CountryCode,
            Name = country.Name
        };
}

using Riok.Mapperly.Abstractions;
using Application.DTOs.Address;
using Domain.Models;

namespace Application.Mappers;

[Mapper]
[UseStaticMapper(typeof(CountryMapper))]
public static partial class AddressMapper
{
    [MapperIgnoreSource(nameof(Address.AddressId))]
    [MapperIgnoreSource(nameof(Address.CountryCode))]
    [MapProperty(nameof(Address.Country), nameof(ResultAddressDTO.Country))]
    [MapProperty(nameof(Address.State), nameof(ResultAddressDTO.State))]
    [MapProperty(nameof(Address.City), nameof(ResultAddressDTO.City))]
    [MapProperty(nameof(Address.Street), nameof(ResultAddressDTO.Street))]
    public static partial ResultAddressDTO ToDTO(Address address);
}

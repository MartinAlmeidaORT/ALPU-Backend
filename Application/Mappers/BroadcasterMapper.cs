namespace Application.Mappers;

using Application.DTOs.Auth;
using Application.DTOs.Users;
using Domain.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
[UseStaticMapper(typeof(AddressMapper))]
[UseStaticMapper(typeof(UserMapper))]
public static partial class BroadcasterMapper
{
    [MapperIgnoreSource(nameof(User.Password))]
    [MapperIgnoreSource(nameof(User.RUT))]
    [MapperIgnoreSource(nameof(User.AddressId))]
    [MapperIgnoreSource(nameof(User.Notifications))]
    [MapperIgnoreSource(nameof(Broadcaster.CategoryId))]
    [MapperIgnoreSource(nameof(Broadcaster.Demos))]
    [MapperIgnoreSource(nameof(Broadcaster.Memberships))]
    [MapperIgnoreSource(nameof(Broadcaster.Contracts))]
    [MapProperty(nameof(Broadcaster.Category.Name), nameof(ResultBroadcasterDTO.BroadcasterCategory))]
    public static partial ResultBroadcasterDTO ToDTO(Broadcaster broadcaster);

    public static Broadcaster ToEntity(CreateBroadcasterDTO dto, Country country) => new()
    {
        Email = dto.Email,
        Password = dto.Password,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        RUT = dto.RUT,
        Address = new()
        {
            Country = country,
            State = dto.State,
            City = dto.City,
            Street = dto.Street,
        }
    };

    public static Broadcaster ToEntity(RegisterBroadcasterGoogleDTO dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        RUT = dto.RUT,
        Address = new()
        {
            CountryCode = dto.CountryCode,
            State = dto.State,
            City = dto.City,
            Street = dto.Street,
        }
    };
}

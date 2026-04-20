using Riok.Mapperly.Abstractions;
using Application.DTOs.Users;
using Domain.Models;
using System.Linq.Expressions;
using Application.DTOs.Address;
using Application.DTOs.Country;
using Domain.Enums;

namespace Application.Mappers;

[Mapper]
[UseStaticMapper(typeof(CountryMapper))]
[UseStaticMapper(typeof(AddressMapper))]
[UseStaticMapper(typeof(ClientMapper))]
[UseStaticMapper(typeof(BroadcasterMapper))]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.Password))]
    [MapperIgnoreSource(nameof(User.RUT))]
    [MapperIgnoreSource(nameof(User.AddressId))]
    [MapperIgnoreSource(nameof(User.Notifications))]
    [MapDerivedType(typeof(Client), typeof(ResultClientDTO))]
    [MapDerivedType(typeof(Broadcaster), typeof(ResultBroadcasterDTO))]
    public static partial ResultUserDTO ToDTO(User user);

    public static User ToEntity(CreateUserDTO dto, Country country) => new()
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

    public static void ApplyUpdate(User user, UpdateUserDTO dto, Country? country)
    {
        if (dto.Name != null) user.FirstName = dto.Name;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (country != null) user.Address.Country = country;
        if (dto.State != null) user.Address.State = dto.State;
        if (dto.City != null) user.Address.City = dto.City;
        if (dto.Street != null) user.Address.Street = dto.Street;
        if (dto.UserState != null) user.UserState = (UserState)dto.UserState;
    }

    public static void ApplyUpdate(Broadcaster broadcaster, UpdateBroadcasterDTO dto, Country? country)
    {
        ApplyUpdate(broadcaster, dto, country);
    }

    public static void ApplyUpdate(Client client, UpdateClientDTO dto, Country? country)
    {
        ApplyUpdate((User)client, dto, country);
    }

    public static Expression<Func<User, IResultUserDTO>> ToDTOExpression() => user =>
        user is Client
            ? new ResultClientDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = new ResultAddressDTO
                {
                    Country = new ResultCountryDTO
                    {
                        CountryCode = user.Address.Country.CountryCode,
                        Name = user.Address.Country.Name
                    },
                    State = user.Address.State,
                    City = user.Address.City,
                    Street = user.Address.Street,
                },
                AgencyName = ((Client)user).Agency.Name
            }
            : new ResultBroadcasterDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = new ResultAddressDTO
                {
                    Country = new ResultCountryDTO
                    {
                        CountryCode = user.Address.Country.CountryCode,
                        Name = user.Address.Country.Name
                    },
                    State = user.Address.State,
                    City = user.Address.City,
                    Street = user.Address.Street,
                },
                BroadcasterCategory = ((Broadcaster)user).Category.Name
            };
}

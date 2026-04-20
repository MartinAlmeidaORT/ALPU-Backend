using Application.DTOs.Users;
using Domain.Models;
using Riok.Mapperly.Abstractions;

namespace Application.Mappers;

[Mapper]
[UseStaticMapper(typeof(AddressMapper))]
[UseStaticMapper(typeof(UserMapper))]
public static partial class ClientMapper
{
    [MapperIgnoreSource(nameof(User.Password))]
    [MapperIgnoreSource(nameof(User.RUT))]
    [MapperIgnoreSource(nameof(User.AddressId))]
    [MapperIgnoreSource(nameof(User.Notifications))]
    [MapperIgnoreSource(nameof(Client.AgencyId))]
    [MapperIgnoreSource(nameof(Client.Contracts))]
    [MapProperty(nameof(Client.Agency.Name), nameof(ResultClientDTO.AgencyName))]
    public static partial ResultClientDTO ToDTO(Client client);


    public static Client ToEntity(CreateClientDTO dto) => new()
    {
        Email = dto.Email,
        Password = dto.Password,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        RUT = dto.RUT,
        Address = new()
        {
            State = dto.State,
            City = dto.City,
            Street = dto.Street
        }
    };
}

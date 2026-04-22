using Domain.Models;
using Application.Interfaces.Public.Services;
using Application.DTOs.Users;
using Application.Mappers;
using Domain.Interfaces.Public.Repositories;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    public IQueryable<IResultUserDTO> GetAllUsers() => unitOfWork.Users.GetAllUsers().Select(UserMapper.ToDTOExpression());

    public async Task<IResultUserDTO?> GetUserByIdAsync(int id) => await unitOfWork.Users.GetUserByIdAsync(id) is User user ? UserMapper.ToDTO(user) : null;

    public async Task<ResultClientDTO> CreateClientAsync(CreateClientDTO dto)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");

        Client client = ClientMapper.ToEntity(dto);

        Agency agency = unitOfWork.Clients.CreateAgency(new(dto.AgencyName));

        client.Agency = agency;
        client.Address.Country = country;

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();
        return ClientMapper.ToDTO(client);
    }

    public async Task<ResultBroadcasterDTO> CreateBroadcasterAsync(CreateBroadcasterDTO dto)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");
        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1) ?? throw new KeyNotFoundException($"Category with id {1} not found.");
        Broadcaster broadcaster = BroadcasterMapper.ToEntity(dto, country);
        broadcaster.Address.Country = country;
        broadcaster.Category = category;
        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();
        return BroadcasterMapper.ToDTO(broadcaster);
    }

    public async Task<IResultUserDTO> UpdateUserAsync(int id, UpdateUserDTO dto)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");

        Country? country = null;
        if (dto.CountryCode != null)
        {
            country = await unitOfWork.Countries.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");
        }

        UserMapper.ApplyUpdate(user, dto, country);

        await unitOfWork.SaveChangesAsync();
        return UserMapper.ToDTO(user);
    }

    public async Task<IResultUserDTO> DeleteUserAsync(int id)
    {
        User? user = await unitOfWork.Users.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");
        unitOfWork.Users.DeleteUser(user);
        await unitOfWork.SaveChangesAsync();
        return UserMapper.ToDTO(user);
    }
}

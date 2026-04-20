using Domain.Models;
using Domain.Interfaces.Public.Repositories;
using Application.Interfaces.Public.Services;
using Application.DTOs.Users;
using Domain.Enums;
using Application.Mappers;

namespace Application.Services;

public class UserService(
    IUserRepository repo,
    IBroadcasterRepository broadcasterRepo,
    IClientRepository clientRepo,
    ICountryRepository countryRepo,
    IUnitOfWork unitOfWork)
: IUserService
{
    public IQueryable<IResultUserDTO> GetAllUsers() => repo.GetAll().Select(UserMapper.ToDTOExpression());

    public async Task<IResultUserDTO?> GetUserByIdAsync(int id) => await repo.GetByIdAsync(id) is User user ? UserMapper.ToDTO(user) : null;

    public async Task<ResultClientDTO> CreateClientAsync(CreateClientDTO dto)
    {
        Country? country = await countryRepo.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");

        Client client = ClientMapper.ToEntity(dto);

        Agency agency = await clientRepo.CreateAgency(new(dto.AgencyName));

        client.Agency = agency;
        client.Address.Country = country;
        await repo.Create(client);
        await unitOfWork.SaveChangesAsync();
        return ClientMapper.ToDTO(client);
    }

    public async Task<ResultBroadcasterDTO> CreateBroadcasterAsync(CreateBroadcasterDTO dto)
    {
        Country? country = await countryRepo.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");
        BroadcasterCategory? category = await broadcasterRepo.GetCategoryByIdAsync(1) ?? throw new KeyNotFoundException($"Category with id {1} not found.");
        Broadcaster broadcaster = BroadcasterMapper.ToEntity(dto, country);
        broadcaster.Address.Country = country;
        broadcaster.Category = category;
        await repo.Create(broadcaster);
        await unitOfWork.SaveChangesAsync();
        return BroadcasterMapper.ToDTO(broadcaster);
    }

    public async Task<IResultUserDTO> UpdateUserAsync(int id, UpdateUserDTO dto)
    {
        User? user = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");

        Country? country = null;
        if (dto.CountryCode != null)
        {
            country = await countryRepo.GetByCodeAsync(dto.CountryCode) ?? throw new KeyNotFoundException($"Country with code {dto.CountryCode} not found.");
        }

        UserMapper.ApplyUpdate(user, dto, country);

        await unitOfWork.SaveChangesAsync();
        return UserMapper.ToDTO(user);
    }

    public async Task<IResultUserDTO> DeleteUserAsync(int id)
    {
        User? user = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User with id {id} not found.");
        await repo.Delete(user);
        await unitOfWork.SaveChangesAsync();
        return UserMapper.ToDTO(user);
    }
}

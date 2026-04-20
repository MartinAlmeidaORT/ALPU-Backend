using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IClientRepository : IUserRepository
{
    public Task<Agency> CreateAgency(Agency entity);

    public Task<Agency?> GetAgencyByIdAsync(int id);
}

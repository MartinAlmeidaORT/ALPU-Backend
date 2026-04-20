using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IBroadcasterRepository : IUserRepository
{
    public Task<BroadcasterCategory?> GetCategoryByIdAsync(int id);
}

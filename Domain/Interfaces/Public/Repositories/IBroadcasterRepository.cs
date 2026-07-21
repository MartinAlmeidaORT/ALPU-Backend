using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IBroadcasterRepository
{
    public Broadcaster CreateBroadcaster(Broadcaster entity);

    public IQueryable<Broadcaster> GetAllBroadcasters();

    public Task<Broadcaster?> GetBroadcasterByIdAsync(int id);

    public Task<Broadcaster?> GetBroadcasterWithSkillsAndLanguagesAsync(int id);

    public Task<BroadcasterCategory?> GetCategoryByIdAsync(int id);

    public Broadcaster UpdateBroadcaster(Broadcaster entity);

    public Broadcaster DeleteBroadcaster(Broadcaster entity);
}

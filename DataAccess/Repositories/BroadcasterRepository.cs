using DataAccess.EntityFramework;
using Domain.Classes.Abstracts;
using Domain.Models;

namespace DataAccess.Repositories;

public class BroadcasterRepository(DatabaseContext context) : RepositoryBase<Broadcaster>(context), IBroadcasterRepository
{
    public Broadcaster CreateBroadcaster(Broadcaster entity) => Create(entity);

    public IQueryable<Broadcaster> GetAllBroadcasters() => GetAll();

    public async Task<Broadcaster?> GetBroadcasterByIdAsync(int id) => await Get(id);

    public async Task<BroadcasterCategory?> GetCategoryByIdAsync(int id) => await context.BroadcasterCategories.FindAsync(id);

    public Broadcaster UpdateBroadcaster(Broadcaster entity) => Update(entity);

    public Broadcaster DeleteBroadcaster(Broadcaster entity) => Delete(entity);
}

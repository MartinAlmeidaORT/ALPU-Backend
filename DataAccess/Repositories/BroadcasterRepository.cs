using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BroadcasterRepository(DatabaseContext context) : RepositoryBase<Broadcaster>(context), IBroadcasterRepository
{
    public Broadcaster CreateBroadcaster(Broadcaster entity) => Create(entity);

    public IQueryable<Broadcaster> GetAllBroadcasters() => GetAll();

    public async Task<Broadcaster?> GetBroadcasterByIdAsync(int id) => await Get(id);

    public async Task<Broadcaster?> GetBroadcasterWithSkillsAndLanguagesAsync(int id) =>
        await GetAll()
            .Include(b => b.Skills)
            .Include(b => b.Languages)
            .Include(b => b.Demos)
            .Include(b => b.Address)
            .Include(b => b.Category)
            .AsSplitQuery()
            .SingleOrDefaultAsync(b => b.UserId == id);

    public async Task<BroadcasterCategory?> GetCategoryByIdAsync(int id) => await context.BroadcasterCategories.FindAsync(id);

    public Broadcaster UpdateBroadcaster(Broadcaster entity) => Update(entity);

    public Broadcaster DeleteBroadcaster(Broadcaster entity) => Delete(entity);
}

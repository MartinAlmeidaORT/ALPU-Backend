using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;

namespace DataAccess.Repositories;

public class UnitOfWork(DatabaseContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}

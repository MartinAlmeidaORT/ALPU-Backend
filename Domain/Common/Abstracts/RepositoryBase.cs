using Microsoft.EntityFrameworkCore;

namespace Domain.Common.Abstracts;

public abstract class RepositoryBase<TEntity>(DbContext context)
    where TEntity : Entity
{
    protected readonly DbContext _context = context;

    protected TEntity Create(TEntity entity) => _context.Set<TEntity>().Add(entity).Entity;

    protected IQueryable<TEntity> GetAll() => _context.Set<TEntity>();

    protected async Task<TEntity?> Get(params object[] keys) => await _context.Set<TEntity>().FindAsync(keys);

    protected TEntity Update(TEntity entity) => _context.Set<TEntity>().Update(entity).Entity;

    protected TEntity Delete(TEntity entity) => _context.Set<TEntity>().Remove(entity).Entity;
}

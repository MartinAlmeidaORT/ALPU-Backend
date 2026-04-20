namespace Domain.Interfaces.Public.Repositories;

public interface IRepository<T>
{
    public IQueryable<T> GetAll();

    public Task<T?> GetByIdAsync(int id);

    public Task<T> Create(T entity);

    public Task<T> Update(T entity);

    public Task<T> Delete(T entity);
}

namespace Domain.Interfaces.Public.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}

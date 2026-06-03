namespace Domain.Interfaces.Public.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IBroadcasterRepository Broadcasters { get; }
    IClientRepository Clients { get; }
    ICountryRepository Countries { get; }
    IDepartmentRepository Departments { get; }
    IContractRepository Contracts { get; }
    IAlpuServiceRepository Services { get; }
    IBillRepository Bills { get; }

    void Attach<T>(T entity) where T : class;
    Task SaveChangesAsync();
}

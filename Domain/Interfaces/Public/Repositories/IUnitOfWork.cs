namespace Domain.Interfaces.Public.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IBroadcasterRepository Broadcasters { get; }
    IClientRepository Clients { get; }
    ICountryRepository Countries { get; }
    IContractRepository Contracts { get; }
    IAlpuServiceRepository Services { get; }

    Task SaveChangesAsync();
}

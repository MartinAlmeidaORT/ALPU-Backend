using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;

namespace DataAccess.Repositories;

public class UnitOfWork(DatabaseContext context) : IUnitOfWork
{
    private readonly DatabaseContext _context = context;
    private IUserRepository? _users;
    private IBroadcasterRepository? _broadcasters;
    private IClientRepository? _clients;
    private ICountryRepository? _countries;
    private IAlpuServiceRepository? _services;

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBroadcasterRepository Broadcasters => _broadcasters ??= new BroadcasterRepository(_context);
    public IClientRepository Clients => _clients ??= new ClientRepository(_context);
    public ICountryRepository Countries => _countries ??= new CountryRepository(_context);
    public IAlpuServiceRepository Services => _services ??= new AlpuServiceRepository(_context);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

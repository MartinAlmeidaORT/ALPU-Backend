using DataAccess.EntityFramework;
using Domain.Interfaces.Public.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UnitOfWork(DatabaseContext context) : IUnitOfWork
{
    private readonly DatabaseContext _context = context;

    private IUserRepository? _users;
    private IBroadcasterRepository? _broadcasters;
    private IClientRepository? _clients;
    private ICountryRepository? _countries;
    private IDepartmentRepository? _departments;
    private IAlpuServiceRepository? _services;
    private IContractRepository? _contracts;
    private IBillRepository? _bills;

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBroadcasterRepository Broadcasters => _broadcasters ??= new BroadcasterRepository(_context);
    public IClientRepository Clients => _clients ??= new ClientRepository(_context);
    public ICountryRepository Countries => _countries ??= new CountryRepository(_context);
    public IDepartmentRepository Departments => _departments ??= new DepartmentRepository(_context);
    public IAlpuServiceRepository Services => _services ??= new AlpuServiceRepository(_context);
    public IContractRepository Contracts => _contracts ??= new ContractRepository(_context);
    public IBillRepository Bills => _bills ??= new BillRepository(_context);

    public void Attach<T>(T entity) where T : class
    {
        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
            _context.Attach(entity);
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

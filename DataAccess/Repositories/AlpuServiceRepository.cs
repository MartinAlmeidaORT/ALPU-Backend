using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class AlpuServiceRepository(DatabaseContext context) : RepositoryBase<Service>(context), IAlpuServiceRepository
{
    public IQueryable<Service> GetAllServices() => GetAll();

    public async Task<Service?> GetServiceByIdAsync(int id) => await Get(id);
}
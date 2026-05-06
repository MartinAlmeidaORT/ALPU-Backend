using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class AlpuServiceRepository(DatabaseContext context) : RepositoryBase<Service>(context), IAlpuServiceRepository
{
    public IQueryable<Service> GetAllServices() => GetAll();

    public async Task<Service?> GetServiceByIdAsync(int id) => await context.Services
        .Where(s => s.ServiceId == id)
        .Include(s => ((ServiceDuration)s).ServicePrices)
        .OrderBy(s => ((ServiceDuration)s).ServicePrices.OrderBy(sp => sp.DurationId))
        .FirstOrDefaultAsync();
}

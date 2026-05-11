using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IAlpuServiceRepository
{
    public IQueryable<Service> GetAllServices();

    public Task<Service?> GetServiceByIdAsync(int id);
}

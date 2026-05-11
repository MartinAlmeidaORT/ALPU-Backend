using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface IAlpuService
{
    IQueryable<Service> GetAllServices();
}
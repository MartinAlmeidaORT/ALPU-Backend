using Domain.Models.Services;

namespace Domain.Interfaces.Public.Services;

public interface IAlpuService
{
    IQueryable<BaseService> GetAllServices();
}

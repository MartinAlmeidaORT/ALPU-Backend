using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models.Services;

namespace Application.Services;

public class AlpuService(IUnitOfWork unitOfWork) : IAlpuService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<BaseService> GetAllServices() => _unitOfWork.Services.GetAllServices();
}

using Application.Interfaces.Public.Services;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace Application.Services;

public class AlpuServiceService(IUnitOfWork unitOfWork) : IAlpuService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<Service> GetAllServices() => _unitOfWork.Services.GetAllServices();

    public Task<Service?> GetServiceByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
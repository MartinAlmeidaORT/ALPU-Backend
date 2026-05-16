using Domain.Enums;
using Domain.Models.Services;

namespace Domain.Interfaces.Public.Repositories;

public interface IAlpuServiceRepository
{
    public IQueryable<BaseService> GetAllServices();

    public IQueryable<VolumeDiscount> GetAllVolumeDiscounts();

    public IQueryable<MultiServiceDiscount> GetAllMultiServiceDiscounts();

    public IQueryable<PriceAdjustment> GetAllPriceAdjustments();

    public Task<BaseService?> GetServiceByIdAsync(int id);

    public Task<BaseService?> GetByType(ServiceType service, int? period = null);

    public Task<VolumeDiscount?> GetVolumeDiscountAsync(ServiceType type, int quantity);

    public Task<MultiServiceDiscount?> GetMultiMediaDiscountAsync(ServiceType typeA, ServiceType typeB);
}

using Domain.Enums;
using Domain.Models.Services;

namespace Domain.Interfaces.Public.Singletons;

public interface IPriceTable
{
    Task<BaseService?> GetServiceById(int id);

    Task<VolumeDiscount?> GetVolumeDiscountAsync(ServiceType type, int quantity);

    Task<MultiServiceDiscount?> GetMultiServiceDiscountAsync(ServiceType typeA, ServiceType typeB);

    Task<PriceAdjustment?> GetPriceAdjustmentAsync(string key);

    void InvalidateCache();
}

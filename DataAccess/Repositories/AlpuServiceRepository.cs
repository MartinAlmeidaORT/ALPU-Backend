using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Models.Services;

namespace DataAccess.Repositories;

public class AlpuServiceRepository(DatabaseContext context) : RepositoryBase<BaseService>(context), IAlpuServiceRepository
{
    public IQueryable<BaseService> GetAllServices() => GetAll();

    public IQueryable<VolumeDiscount> GetAllVolumeDiscounts() => context.VolumeDiscounts;

    public IQueryable<MultiServiceDiscount> GetAllMultiServiceDiscounts() => context.MultiServiceDiscounts;

    public IQueryable<PriceAdjustment> GetAllPriceAdjustments() => context.PriceAdjustments;

    public async Task<BaseService?> GetServiceByIdAsync(int id) => await Get(id);

    public async Task<BaseService?> GetByType(ServiceType service, int? period = null)
    {
        return context.Services
            .Where(s => s.Type == service)
            .FirstOrDefault();
    }

    public async Task<VolumeDiscount?> GetVolumeDiscountAsync(ServiceType type, int quantity)
    {
        return context.VolumeDiscounts
            .Where(v => v.ServiceType == type
                     && v.MinQuantity <= quantity
                     && (v.MaxQuantity == null || v.MaxQuantity >= quantity))
            .OrderByDescending(d => d.MinQuantity)
            .FirstOrDefault();
    }

    public async Task<MultiServiceDiscount?> GetMultiMediaDiscountAsync(ServiceType typeA, ServiceType typeB)
    {
        return context.MultiServiceDiscounts
            .FirstOrDefault(m =>
                (m.ServiceA == typeA && m.ServiceB == typeB) ||
                (m.ServiceB == typeB && m.ServiceA == typeA));
    }
}

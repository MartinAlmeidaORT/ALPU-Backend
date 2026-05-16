using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Singletons;

public class PriceTable(IServiceScopeFactory scopeFactory) : IPriceTable
{
    private List<BaseService> _services = [];
    private List<VolumeDiscount> _volumeDiscounts = [];
    private List<MultiServiceDiscount> _multiServiceDiscounts = [];
    private List<PriceAdjustment> _priceAdjustments = [];
    private DateTime _lastLoad = DateTime.MinValue;

    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    private async Task EnsureLoadedAsync()
    {
        if (_services.Any() && DateTime.UtcNow - _lastLoad < TimeSpan.FromMinutes(5)) return;

        await _lock.WaitAsync();
        try
        {
            if (_services.Any() && DateTime.UtcNow - _lastLoad < TimeSpan.FromMinutes(5)) return;

            using IServiceScope scope = _scopeFactory.CreateScope();
            IUnitOfWork uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _services = [.. uow.Services.GetAllServices()];
            _volumeDiscounts = [.. uow.Services.GetAllVolumeDiscounts()];
            _multiServiceDiscounts = [.. uow.Services.GetAllMultiServiceDiscounts()];
            _priceAdjustments = [.. uow.Services.GetAllPriceAdjustments()];
            _lastLoad = DateTime.UtcNow;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<BaseService?> GetServiceById(int id)
    {
        await EnsureLoadedAsync();

        return _services.FirstOrDefault(s => s.ServiceId == id);
    }

    public async Task<VolumeDiscount?> GetVolumeDiscountAsync(ServiceType type, int quantity)
    {
        await EnsureLoadedAsync();

        return _volumeDiscounts
            .Where(v => v.ServiceType == type
                     && v.MinQuantity <= quantity
                     && (v.MaxQuantity == null || v.MaxQuantity >= quantity))
            .OrderByDescending(d => d.MinQuantity)
            .FirstOrDefault();
    }

    public async Task<MultiServiceDiscount?> GetMultiServiceDiscountAsync(ServiceType typeA, ServiceType typeB)
    {
        await EnsureLoadedAsync();

        return _multiServiceDiscounts
            .FirstOrDefault(m =>
                (m.ServiceA == typeA && m.ServiceB == typeB) ||
                (m.ServiceB == typeB && m.ServiceA == typeA));
    }

    public async Task<PriceAdjustment?> GetPriceAdjustmentAsync(string key)
    {
        await EnsureLoadedAsync();

        return _priceAdjustments.FirstOrDefault(p => p.Key == key);
    }

    // Llamado desde el panel del admin cuando edita una tarifa (RF17)
    public void InvalidateCache() => _lastLoad = DateTime.MinValue;
}

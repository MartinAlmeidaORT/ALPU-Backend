using Application.Singletons;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Tests.Common;

public class PriceTableTests
{
    private static IPriceTable CreatePriceTable(
        List<BaseService>? services = null,
        List<VolumeDiscount>? discounts = null,
        List<MultiServiceDiscount>? multiDiscounts = null,
        List<PriceAdjustment>? adjustments = null)
    {
        IServiceScopeFactory scopeFactory = Substitute.For<IServiceScopeFactory>();
        IServiceScope scope = Substitute.For<IServiceScope>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IAlpuServiceRepository serviceRepo = Substitute.For<IAlpuServiceRepository>();

        scopeFactory.CreateScope().Returns(scope);
        scope.ServiceProvider.GetService(typeof(IUnitOfWork)).Returns(unitOfWork);
        unitOfWork.Services.Returns(serviceRepo);

        serviceRepo.GetAllServices()
            .Returns((services ?? []).AsQueryable());
        serviceRepo.GetAllVolumeDiscounts()
            .Returns((discounts ?? []).AsQueryable());
        serviceRepo.GetAllMultiServiceDiscounts()
            .Returns((multiDiscounts ?? []).AsQueryable());
        serviceRepo.GetAllPriceAdjustments()
            .Returns((adjustments ?? []).AsQueryable());

        return new PriceTable(scopeFactory);
    }

    [Fact]
    public async Task GetVolumeDiscountAsync_QuantityInRange_ReturnsDiscount()
    {
        var discounts = new List<VolumeDiscount>
        {
            new() { ServiceType = ServiceType.TvGeneric, MinQuantity = 4, MaxQuantity = null, Type = PriceAdjustmentType.Percentage, Amount = 0.10m },
        };
        var priceTable = CreatePriceTable(discounts: discounts); // ← named argument

        VolumeDiscount? result = await priceTable.GetVolumeDiscountAsync(ServiceType.TvGeneric, 4);

        Assert.NotNull(result);
        Assert.Equal(0.10m, result.Amount);
    }

    // igual para los otros tres tests de VolumeDiscount — agregar discounts:

    [Fact]
    public async Task GetMultiServiceDiscountAsync_AAndB_ReturnsDiscount()
    {
        var multiDiscounts = new List<MultiServiceDiscount>
        {
            new() { ServiceA = ServiceType.TvGeneric, ServiceB = ServiceType.RadioGeneric, Type = PriceAdjustmentType.Percentage, Amount = 0.50m }
        };
        var priceTable = CreatePriceTable(multiDiscounts: multiDiscounts); // ← usa el helper

        MultiServiceDiscount? result = await priceTable.GetMultiServiceDiscountAsync(ServiceType.TvGeneric, ServiceType.RadioGeneric);

        Assert.NotNull(result);
        Assert.Equal(0.50m, result.Amount);
    }

    [Fact]
    public async Task GetMultiServiceDiscountAsync_BAndA_ReturnsDiscount()
    {
        var multiDiscounts = new List<MultiServiceDiscount>
        {
            new() { ServiceA = ServiceType.TvGeneric, ServiceB = ServiceType.RadioGeneric, Type = PriceAdjustmentType.Percentage, Amount = 0.50m }
        };
        var priceTable = CreatePriceTable(multiDiscounts: multiDiscounts); // ← usa el helper

        MultiServiceDiscount? result = await priceTable.GetMultiServiceDiscountAsync(ServiceType.RadioGeneric, ServiceType.TvGeneric);

        Assert.NotNull(result);
        Assert.Equal(0.50m, result.Amount);
    }
}

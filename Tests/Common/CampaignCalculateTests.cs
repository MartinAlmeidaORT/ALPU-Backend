using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Singletons;
using Domain.Models;
using Domain.Models.Campaign;
using Domain.Models.Services;
using FluentResults;
using NSubstitute;
using Tests.Helpers;

namespace Tests.Common;

public class CampaignCalculateTests
{
    private static CampaignInput CreateInput(int broadcasterId) => new()
    {
        ClientId = 1,
        BroadcasterId = broadcasterId,
        Campaign = "Test",
        Services = [],
        CountryCode = "UY"
    };

    private static BaseCampaignService CreateMockService(
        ServiceType type, decimal subtotal)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        priceTable.GetVolumeDiscountAsync(type, Arg.Any<int>())
            .Returns((VolumeDiscount?)null);

        var service = new GenericService(1, "Test", type, subtotal);
        var campaignService = Substitute.For<BaseCampaignService>(
            service, new List<Piece> { new() { Name = "Pieza 1" } }, priceTable);

        campaignService.Calculate(Arg.Any<CampaignInput>()).Returns(Result.Ok(new ServiceBreakdown
        {
            ServiceName = service.Name,
            ServiceType = type,
            SubTotal = subtotal,
        }));

        return campaignService;
    }

    private static IPriceTable CreatePriceTable(
        List<MultiServiceDiscount>? multiDiscounts = null,
        List<PriceAdjustment>? adjustments = null)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();

        priceTable.GetMultiServiceDiscountAsync(Arg.Any<ServiceType>(), Arg.Any<ServiceType>())
            .Returns((MultiServiceDiscount?)null);
        priceTable.GetPriceAdjustmentAsync(Arg.Any<string>())
            .Returns((PriceAdjustment?)null);

        if (multiDiscounts != null)
            foreach (var d in multiDiscounts)
                priceTable.GetMultiServiceDiscountAsync(d.ServiceA, d.ServiceB)
                    .Returns(d);

        if (adjustments != null)
            foreach (var a in adjustments)
                priceTable.GetPriceAdjustmentAsync(a.Key)
                    .Returns(a);

        return priceTable;
    }

    private static IUnitOfWork CreateUnitOfWork(int broadcasterCategoryId)
    {
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IBroadcasterRepository broadcasterRepo = Substitute.For<IBroadcasterRepository>();
        unitOfWork.Broadcasters.Returns(broadcasterRepo);

        // En lugar de mockear Broadcaster, mockeamos solo el repo
        broadcasterRepo.GetBroadcasterByIdAsync(Arg.Any<int>())
            .Returns((Broadcaster?)null);

        // Sobrescribir con categoryId específico usando un stub
        if (broadcasterCategoryId > 0)
        {
            var broadcaster = new Broadcaster
            {
                CategoryId = broadcasterCategoryId
            };
            broadcasterRepo.GetBroadcasterByIdAsync(Arg.Any<int>())
                .Returns(broadcaster);
        }

        return unitOfWork;
    }

    [Fact]
    public async Task Calculate_SingleService_ReturnsTotalEqualToSubtotal()
    {
        var services = new List<BaseCampaignService>
        {
            CreateMockService(ServiceType.TvGeneric, 5000)
        };
        var campaign = new Campaign("Test", services);

        Result<PriceBreakdown> result = await campaign.Calculate(
            CreateInput(1), CreatePriceTable(), CreateUnitOfWork(2));

        Assert.True(result.IsSuccess);
        Assert.Equal(5000, result.Value.Total);
    }

    [Fact]
    public async Task Calculate_MultiServiceDiscount_ReducesTotal()
    {
        var multiDiscounts = new List<MultiServiceDiscount>
        {
            new() { ServiceA = ServiceType.TvGeneric, ServiceB = ServiceType.RadioGeneric, Type = PriceAdjustmentType.Percentage, Amount = 0.50m, IsDiscountForServiceBOnly = true }
        };
        var services = new List<BaseCampaignService>
        {
            CreateMockService(ServiceType.TvGeneric, 5000),
            CreateMockService(ServiceType.RadioGeneric, 3000),
        };
        var campaign = new Campaign("Test", services);

        Result<PriceBreakdown> result = await campaign.Calculate(
            CreateInput(1), CreatePriceTable(multiDiscounts: multiDiscounts), CreateUnitOfWork(2));

        Assert.True(result.IsSuccess);
        Assert.Equal(6500, result.Value.Total); // 5000 + 3000 - (3000 * 0.50)
    }

    [Fact]
    public async Task Calculate_NewBroadcaster_AppliesAdjustment()
    {
        var adjustments = new List<PriceAdjustment>
        {
            new() { Key = "new_broadcaster", Type = PriceAdjustmentType.Percentage, Amount = 0.50m, Name = "Locutor novel" }
        };
        var services = new List<BaseCampaignService>
        {
            CreateMockService(ServiceType.TvGeneric, 5000)
        };
        var campaign = new Campaign("Test", services);

        Result<PriceBreakdown> result = await campaign.Calculate(
            CreateInput(1), CreatePriceTable(adjustments: adjustments), CreateUnitOfWork(1));

        Assert.True(result.IsSuccess);
        Assert.Equal(2500, result.Value.Total); // 5000 - (5000 * 0.20)
    }

    [Fact]
    public async Task Calculate_ServiceCalculateFails_ReturnsFail()
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        var service = Substitute.For<BaseCampaignService>(
            new GenericService(1, "Test", ServiceType.TvGeneric, 1000),
            new List<Piece>(),
            priceTable);
        service.Calculate(Arg.Any<CampaignInput>()).Returns(Result.Fail("Error en el servicio"));

        var campaign = new Campaign("Test", [service]);

        Result<PriceBreakdown> result = await campaign.Calculate(
            CreateInput(1), CreatePriceTable(), CreateUnitOfWork(2));

        Assert.True(result.IsFailed);
        Assert.Contains("Error en el servicio", result.Errors[0].Message);
    }
}

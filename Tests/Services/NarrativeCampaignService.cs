using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Services;
using FluentResults;
using NSubstitute;

namespace Tests.Services;

public class NarrativeCampaignServiceTests
{
    private static NarrativeService CreateService(
        decimal basePrice,
        decimal extraPrice,
        decimal rolePrice) => new NarrativeService
        {
            ServiceId = 1,
            Name = "Narrative Test",
            Type = ServiceType.Narrative,
            BasePrice = basePrice,
            ExtraPrice = extraPrice,
            RolePrice = rolePrice,
        };

    private static NarrativeCampaignService CreateCampaignService(
        NarrativeService service,
        NarrativeCampaignServiceOptions options)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        priceTable.GetVolumeDiscountAsync(Arg.Any<ServiceType>(), Arg.Any<int>())
            .Returns((VolumeDiscount?)null);
        return new NarrativeCampaignService(service, [new Piece { Name = "Narración" }], priceTable, options);
    }

    [Fact]
    public void CalculateSubTotal_NoExtraMinutes_ReturnsBasePrice()
    {
        var service = CreateService(6800, 700, 2050);
        var options = new NarrativeCampaignServiceOptions { Minutes = 0, ExtraRoles = 0 };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(6800, result);
    }

    [Fact]
    public void CalculateSubTotal_WithExtraMinutes_AddsExtraPrice()
    {
        var service = CreateService(6800, 700, 2050);
        var options = new NarrativeCampaignServiceOptions { Minutes = 3, ExtraRoles = 0 };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(6800 + 3 * 700, result); // 6800 + 2100
    }

    [Fact]
    public void CalculateSubTotal_NullBasePrice_ThrowsNullReferenceException()
    {
        var service = CreateService(6800, 700, 2050);
        service.BasePrice = null;
        var options = new NarrativeCampaignServiceOptions { Minutes = 0, ExtraRoles = 0 };
        var campaignService = CreateCampaignService(service, options);

        Assert.Throws<NullReferenceException>(() => campaignService.CalculateSubTotal());
    }

    [Fact]
    public void CalculateSubTotal_NullExtraPrice_ThrowsNullReferenceException()
    {
        var service = CreateService(6800, 700, 2050);
        service.ExtraPrice = null;
        var options = new NarrativeCampaignServiceOptions { Minutes = 3, ExtraRoles = 0 };
        var campaignService = CreateCampaignService(service, options);

        Assert.Throws<NullReferenceException>(() => campaignService.CalculateSubTotal());
    }

    [Fact]
    public async Task Calculate_WithExtraRoles_AddsRolePrice()
    {
        var service = CreateService(6800, 700, 2050);
        var options = new NarrativeCampaignServiceOptions { Minutes = 0, ExtraRoles = 2 };
        var campaignService = CreateCampaignService(service, options);

        Result<ServiceBreakdown> result = await campaignService.Calculate();

        Assert.True(result.IsSuccess);
        Assert.Equal(6800 + 2 * 2050, result.Value.SubTotal); // 6800 + 4100
    }

    [Fact]
    public async Task Calculate_WithPriceOverride_IgnoresCalculation()
    {
        var service = CreateService(6800, 700, 2050);
        var options = new NarrativeCampaignServiceOptions
        {
            Minutes = 5,
            ExtraRoles = 3,
            PriceOverride = 9999
        };
        var campaignService = CreateCampaignService(service, options);

        Result<ServiceBreakdown> result = await campaignService.Calculate();

        Assert.True(result.IsSuccess);
        Assert.Equal(9999, result.Value.SubTotal);
    }
}

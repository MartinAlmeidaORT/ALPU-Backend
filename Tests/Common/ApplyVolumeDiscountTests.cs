using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Services;
using NSubstitute;
using Tests.Helpers;

namespace Tests.Common;

public class ApplyVolumeDiscountTests
{
    private static GenericCampaignService CreateCampaignService(
        decimal basePrice, int pieceCount)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        var service = new GenericService(1, "Test", ServiceType.TvGeneric, basePrice);
        var pieces = Enumerable.Range(1, pieceCount)
            .Select(i => new Piece { Name = $"Pieza {i}" })
            .ToList();
        return new GenericCampaignService(service, pieces, priceTable);
    }

    [Fact]
    public void ApplyVolumeDiscount_Percentage_ReducesSubTotal()
    {
        var campaignService = CreateCampaignService(1000, 1);
        var breakdown = new ServiceBreakdown { SubTotal = 5000 };
        var discount = new VolumeDiscount
        {
            Type = PriceAdjustmentType.Percentage,
            Amount = 0.10m // 10%
        };

        campaignService.ApplyVolumeDiscount(ref breakdown, discount);

        Assert.Equal(4500, breakdown.SubTotal);         // 5000 - 500
        Assert.Single(breakdown.Adjustments);
        Assert.Equal(500, breakdown.Adjustments[0].ApplyDiscount); // descuento aplicado
    }

    [Fact]
    public void ApplyVolumeDiscount_Fixed_ReducesSubTotal()
    {
        var campaignService = CreateCampaignService(1000, 1);
        var breakdown = new ServiceBreakdown { SubTotal = 5000 };
        var discount = new VolumeDiscount
        {
            Type = PriceAdjustmentType.Fixed,
            Amount = 300
        };

        campaignService.ApplyVolumeDiscount(ref breakdown, discount);

        Assert.Equal(4700, breakdown.SubTotal);         // 5000 - 300
        Assert.Single(breakdown.Adjustments);
        Assert.Equal(300, breakdown.Adjustments[0].ApplyDiscount);
    }

    [Fact]
    public void ApplyVolumeDiscount_InvalidType_ThrowsArgumentException()
    {
        var campaignService = CreateCampaignService(1000, 1);
        var breakdown = new ServiceBreakdown { SubTotal = 5000 };
        var discount = new VolumeDiscount
        {
            Type = (PriceAdjustmentType)99,
            Amount = 100
        };

        Assert.Throws<ArgumentException>(() =>
            campaignService.ApplyVolumeDiscount(ref breakdown, discount));
    }
}

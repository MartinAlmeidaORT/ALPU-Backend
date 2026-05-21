using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Services;
using NSubstitute;
using Tests.Helpers;

namespace Tests.Services;

public class BaseCampaignServiceTests
{
    private static BaseService CreateService(
        decimal basePrice,
        decimal? extraPrice = null,
        decimal? firstExtraPrice = null) => new DateService
        {
            ServiceId = 1,
            Name = "Test Service",
            Type = ServiceType.TvGeneric,
            BasePrice = basePrice,
            ExtraPrice = extraPrice,
            FirstExtraPrice = firstExtraPrice,
        };

    private static List<Piece> CreatePieces(int count) => [.. Enumerable.Range(1, count).Select(i => new Piece { Name = $"Pieza {i}" })];

    private static BaseCampaignService CreateCampaignService(
        BaseService service, List<Piece> pieces)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        return new GenericCampaignService(service, pieces, priceTable);
    }

    [Fact]
    public void CalculateSubTotal_OnePiece_ReturnsBasePrice()
    {
        var service = CreateService(basePrice: 1000);
        var campaignService = CreateCampaignService(service, CreatePieces(1));

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(1000, result);
    }

    [Fact]
    public void CalculateSubTotal_TwoPieces_WithFirstExtraPrice_UsesFirstExtraPrice()
    {
        var service = CreateService(basePrice: 1000, extraPrice: 300, firstExtraPrice: 500);
        var campaignService = CreateCampaignService(service, CreatePieces(2));

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(1500, result); // 1000 + 500
    }

    [Fact]
    public void CalculateSubTotal_TwoPieces_WithoutFirstExtraPrice_UsesExtraPrice()
    {
        var service = CreateService(basePrice: 1000, extraPrice: 300);
        var campaignService = CreateCampaignService(service, CreatePieces(2));

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(1300, result); // 1000 + 300
    }

    [Fact]
    public void CalculateSubTotal_ThreePieces_WithFirstExtraPrice_UsesFirstThenExtra()
    {
        var service = CreateService(basePrice: 1000, extraPrice: 300, firstExtraPrice: 500);
        var campaignService = CreateCampaignService(service, CreatePieces(3));

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(1800, result); // 1000 + 500 + 300
    }

    [Fact]
    public void CalculateSubTotal_FourPieces_WithFirstExtraPrice_UsesFirstThenExtraForRest()
    {
        var service = CreateService(basePrice: 1000, extraPrice: 300, firstExtraPrice: 500);
        var campaignService = CreateCampaignService(service, CreatePieces(4));

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(2100, result); // 1000 + 500 + 300 + 300
    }

    [Fact]
    public void CalculateSubTotal_TwoPieces_WithoutExtraPrice_UsesBasePrice()
    {
        var service = CreateService(basePrice: 1000);
        var campaignService = CreateCampaignService(service, CreatePieces(2));

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(2000, result); // 1000 + 1000
    }

    [Fact]
    public void CalculateSubTotal_NullBasePrice_ThrowsNullReferenceException()
    {
        var service = CreateService(basePrice: 0);
        service.BasePrice = null;
        var campaignService = CreateCampaignService(service, CreatePieces(1));

        Assert.Throws<NullReferenceException>(() => campaignService.CalculateSubTotal());
    }
}

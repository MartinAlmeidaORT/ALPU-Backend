using Domain.Common.Inputs.CampaignService;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Services;
using NSubstitute;

namespace Tests.Services;

public class IvrCampaignServiceTests
{
    private static IvrService CreateService(
        decimal basePrice,
        decimal extraPrice,
        decimal updateMessagePrice,
        List<RangeIvr> rangeIvr) => new()
        {
            ServiceId = 1,
            Name = "IVR Test",
            Type = ServiceType.Ivr,
            BasePrice = basePrice,
            ExtraPrice = extraPrice,
            UpdateMessagePrice = updateMessagePrice,
            RangeIvr = rangeIvr,
        };

    private static List<RangeIvr> CreateRangeIvr() =>
    [
        new() { MinWord = 1,   MaxWord = 100, PricePerWord = 21 },
        new() { MinWord = 101, MaxWord = 200, PricePerWord = 19 },
        new() { MinWord = 201, MaxWord = null, PricePerWord = 17 },
    ];

    private static IvrCampaignService CreateCampaignService(
        IvrService service,
        IvrCampaignServiceOptions options)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        return new IvrCampaignService(service, [new Piece { Name = "IVR" }], priceTable, options);
    }

    [Fact]
    public void CalculateSubTotal_WordsInFirstRange_UsesFirstRangePrice()
    {
        var service = CreateService(8200, 4100, 3100, CreateRangeIvr());
        var options = new IvrCampaignServiceOptions
        {
            MessageText = string.Join(" ", Enumerable.Repeat("word", 50)), // 50 palabras
            AdditionalMessages = 0,
            CanUpdate = false,
            IsInterior = false,
        };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(8200 + 50 * 21, result); // basePrice + 50 palabras * 21
    }

    [Fact]
    public void CalculateSubTotal_WordsInSecondRange_UsesSecondRangePrice()
    {
        var service = CreateService(8200, 4100, 3100, CreateRangeIvr());
        var options = new IvrCampaignServiceOptions
        {
            MessageText = string.Join(" ", Enumerable.Repeat("word", 150)), // 150 palabras
            AdditionalMessages = 0,
            CanUpdate = false,
            IsInterior = false,
        };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(8200 + 150 * 19, result);
    }

    [Fact]
    public void CalculateSubTotal_WordsInThirdRange_UsesThirdRangePrice()
    {
        var service = CreateService(8200, 4100, 3100, CreateRangeIvr());
        var options = new IvrCampaignServiceOptions
        {
            MessageText = string.Join(" ", Enumerable.Repeat("word", 250)), // 250 palabras
            AdditionalMessages = 0,
            CanUpdate = false,
            IsInterior = false,
        };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(8200 + 250 * 17, result);
    }

    [Fact]
    public void CalculateSubTotal_WithAdditionalMessages_AddsExtraPrice()
    {
        var service = CreateService(8200, 4100, 3100, CreateRangeIvr());
        var options = new IvrCampaignServiceOptions
        {
            MessageText = string.Join(" ", Enumerable.Repeat("word", 50)),
            AdditionalMessages = 2,
            CanUpdate = false,
            IsInterior = false,
        };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(8200 + 50 * 21 + 2 * 4100, result);
    }

    [Fact]
    public void CalculateSubTotal_EmptyText_ReturnsBasePrice()
    {
        var service = CreateService(8200, 4100, 3100, CreateRangeIvr());
        var options = new IvrCampaignServiceOptions
        {
            MessageText = "",
            AdditionalMessages = 0,
            CanUpdate = false,
            IsInterior = false,
        };
        var campaignService = CreateCampaignService(service, options);

        decimal result = campaignService.CalculateSubTotal();

        Assert.Equal(8200, result);
    }

    [Fact]
    public void CalculateSubTotal_NullBasePrice_ThrowsNullReferenceException()
    {
        var service = CreateService(8200, 4100, 3100, CreateRangeIvr());
        service.BasePrice = null;
        var options = new IvrCampaignServiceOptions
        {
            MessageText = "test",
            AdditionalMessages = 0,
            CanUpdate = false,
            IsInterior = false,
        };
        var campaignService = CreateCampaignService(service, options);

        Assert.Throws<NullReferenceException>(() => campaignService.CalculateSubTotal());
    }
}

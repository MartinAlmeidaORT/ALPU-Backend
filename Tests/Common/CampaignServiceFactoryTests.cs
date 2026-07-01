using System.Text.Json;
using Application.Factories;
using Domain.Common.Inputs.CampaignService;
using Domain.Enums;
using Domain.Interfaces.Public.Singletons;
using Domain.Models.Campaign;
using Domain.Models.Campaign.Period;
using Domain.Models.Services;
using FluentResults;
using NSubstitute;

namespace Tests.Common;

public class CampaignServiceFactoryTests
{
    private static IPriceTable CreatePriceTable(BaseService? service = null)
    {
        IPriceTable priceTable = Substitute.For<IPriceTable>();
        priceTable.GetServiceById(Arg.Any<int>()).Returns(service);
        return priceTable;
    }

    private static CampaignServiceInput CreateInput(
        int serviceId,
        string options,
        List<PieceInput>? pieces = null) => new()
        {
            ServiceId = serviceId,
            Options = JsonDocument.Parse(options).RootElement,
            Pieces = pieces ?? [new PieceInput { Name = "Pieza 1" }]
        };

    [Fact]
    public async Task Create_ServiceNotFound_ReturnsFail()
    {
        var factory = new CampaignServiceFactory(CreatePriceTable(null));

        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(999, "{}"));

        Assert.True(result.IsFailed);
        Assert.Contains("999", result.Errors[0].Message);
    }

    [Fact]
    public async Task Create_TvService_ReturnsSuccess()
    {
        var service = new PeriodService
        {
            ServiceId = 1,
            Name = "TELEVISIÓN",
            Type = ServiceType.TvGeneric,
            Periods = [new() { Interval = Interval.OneWeek, BasePrice = 14700, ExtraPrice = 5900 }]
        };
        var factory = new CampaignServiceFactory(CreatePriceTable(service));

        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(1, """{ "period": "ONE_WEEK", "isInterior": false }"""));

        Assert.True(result.IsSuccess);
        Assert.IsType<TvCampaignService>(result.Value);
    }

    [Fact]
    public async Task Create_InvalidOptions_ReturnsFail()
    {
        var service = new PeriodService
        {
            ServiceId = 1,
            Name = "TELEVISIÓN",
            Type = ServiceType.TvGeneric,
            Periods = [new() { Interval = Interval.OneWeek, BasePrice = 14700 }]
        };
        var factory = new CampaignServiceFactory(CreatePriceTable(service));

        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(1, """{ "period": "ONE_WEEK", "unknownOption": true }"""));

        Assert.True(result.IsFailed);
        Assert.Contains("unknownOption", result.Errors[0].Message);
    }

    [Fact]
    public async Task Create_MissingRequiredOption_ReturnsFail()
    {
        var service = new PeriodService
        {
            ServiceId = 1,
            Name = "TELEVISIÓN",
            Type = ServiceType.TvGeneric,
            Periods = [new() { Interval = Interval.OneWeek, BasePrice = 14700 }]
        };
        var factory = new CampaignServiceFactory(CreatePriceTable(service));

        // Falta period que es requerido
        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(1, """{ "isInterior": false }"""));

        Assert.True(result.IsFailed);
        Assert.Contains("period", result.Errors[0].Message);
    }

    [Fact]
    public async Task Create_NarrativeService_ReturnsSuccess()
    {
        var service = new NarrativeService
        {
            ServiceId = 2,
            Name = "NARRACIONES",
            Type = ServiceType.Narrative,
            BasePrice = 6800,
            ExtraPrice = 700,
            RolePrice = 2050
        };
        var factory = new CampaignServiceFactory(CreatePriceTable(service));

        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(2, """{ "minutes": 3, "extraRoles": 0 }"""));

        Assert.True(result.IsSuccess);
        Assert.IsType<NarrativeCampaignService>(result.Value);
    }

    [Fact]
    public async Task Create_IvrService_ReturnsSuccess()
    {
        var service = new IvrService
        {
            ServiceId = 3,
            Name = "IVR",
            Type = ServiceType.Ivr,
            BasePrice = 8200,
            ExtraPrice = 4100,
            UpdateMessagePrice = 3100,
            RangeIvr =
            [
                new() { MinWord = 1, MaxWord = 100, PricePerWord = 21 },
                new() { MinWord = 101, MaxWord = null, PricePerWord = 19 }
            ]
        };
        var factory = new CampaignServiceFactory(CreatePriceTable(service));

        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(3, """{ "messageText": "hola mundo", "additionalMessages": 0, "canUpdate": false, "isInterior": false }"""));

        Assert.True(result.IsSuccess);
        Assert.IsType<IvrCampaignService>(result.Value);
    }

    [Fact]
    public async Task Create_FirstPieceWithoutName_ReturnsFail()
    {
        var service = new PeriodService
        {
            ServiceId = 1,
            Name = "TELEVISIÓN",
            Type = ServiceType.TvGeneric,
            Periods = [new() { Interval = Interval.OneWeek, BasePrice = 14700 }]
        };
        var factory = new CampaignServiceFactory(CreatePriceTable(service));

        Result<BaseCampaignService> result = await factory.Create(
            CreateInput(1, """{ "period": "ONE_WEEK", "isInterior": false }""",
                [new PieceInput { Name = "" }]));

        Assert.True(result.IsFailed);
        Assert.Contains("primera pieza", result.Errors[0].Message);
    }
}

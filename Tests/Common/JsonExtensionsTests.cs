using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Extensions;
using Domain.Enums;
using FluentResults;

namespace Tests.Common;

public class ConvertJsonElementToRecordTests
{
    private static JsonElement ParseJson(string json) =>
        JsonDocument.Parse(json).RootElement;

    private record TestOptions
    {
        [JsonRequired]
        public string Name { get; init; } = null!;
        public int Count { get; init; }
    }

    private record TestOptionsWithEnum
    {
        [JsonRequired]
        public Interval Period { get; init; }
        public bool IsInterior { get; init; }
    }

    [Fact]
    public void ConvertJsonElementToRecord_ValidJson_ReturnsSuccess()
    {
        var json = ParseJson("""{ "name": "test", "count": 5 }""");

        Result<TestOptions> result = json.ConvertJsonElementToRecord<TestOptions>();

        Assert.True(result.IsSuccess);
        Assert.Equal("test", result.Value.Name);
        Assert.Equal(5, result.Value.Count);
    }

    [Fact]
    public void ConvertJsonElementToRecord_UnknownKey_ReturnsFail()
    {
        var json = ParseJson("""{ "name": "test", "unknownKey": 5 }""");

        Result<TestOptions> result = json.ConvertJsonElementToRecord<TestOptions>();

        Assert.True(result.IsFailed);
        Assert.Contains("unknownKey", result.Errors[0].Message);
    }

    [Fact]
    public void ConvertJsonElementToRecord_MissingRequiredField_ReturnsFail()
    {
        var json = ParseJson("""{ "count": 5 }""");

        Result<TestOptions> result = json.ConvertJsonElementToRecord<TestOptions>();

        Assert.True(result.IsFailed);
        Assert.Contains("name", result.Errors[0].Message);
    }

    [Fact]
    public void ConvertJsonElementToRecord_ValidEnum_ReturnsSuccess()
    {
        var json = ParseJson("""{ "period": "ONE_WEEK", "isInterior": false }""");

        Result<TestOptionsWithEnum> result = json.ConvertJsonElementToRecord<TestOptionsWithEnum>();

        Assert.True(result.IsSuccess);
        Assert.Equal(Interval.OneWeek, result.Value.Period);
    }

    [Fact]
    public void ConvertJsonElementToRecord_InvalidEnum_ReturnsFail()
    {
        var json = ParseJson("""{ "period": "INVALID_VALUE" }""");

        Result<TestOptionsWithEnum> result = json.ConvertJsonElementToRecord<TestOptionsWithEnum>();

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void ConvertJsonElementToRecord_MissingRequiredEnum_ReturnsFail()
    {
        var json = ParseJson("""{ "isInterior": false }""");

        Result<TestOptionsWithEnum> result = json.ConvertJsonElementToRecord<TestOptionsWithEnum>();

        Assert.True(result.IsFailed);
        Assert.Contains("period", result.Errors[0].Message);
    }

    [Fact]
    public void ConvertJsonElementToRecord_EmptyJson_MissingRequired_ReturnsFail()
    {
        var json = ParseJson("""{}""");

        Result<TestOptions> result = json.ConvertJsonElementToRecord<TestOptions>();

        Assert.True(result.IsFailed);
        Assert.Contains("name", result.Errors[0].Message);
    }
}

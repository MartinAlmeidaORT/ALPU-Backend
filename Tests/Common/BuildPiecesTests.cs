using Application.Factories;
using Domain.Common.Inputs.CampaignService;
using Domain.Models.Campaign;
using FluentResults;

namespace Tests.Common;

public class BuildPiecesTests
{
    private static Result<List<Piece>> BuildPieces(List<PieceInput> inputs) =>
        CampaignServiceFactory.BuildPieces(inputs);

    [Fact]
    public void BuildPieces_EmptyList_ReturnsFail()
    {
        Result<List<Piece>> result = BuildPieces([]);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void BuildPieces_FirstPieceWithoutName_ReturnsFail()
    {
        Result<List<Piece>> result = BuildPieces([new PieceInput { Name = "" }]);

        Assert.True(result.IsFailed);
        Assert.Contains("primera pieza", result.Errors[0].Message);
    }

    [Fact]
    public void BuildPieces_FirstPieceWithName_ReturnsSuccess()
    {
        Result<List<Piece>> result = BuildPieces([new PieceInput { Name = "MyPiece" }]);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("MyPiece", result.Value[0].Name);
    }

    [Fact]
    public void BuildPieces_SubsequentPiecesWithoutName_GeneratesGenericName()
    {
        Result<List<Piece>> result = BuildPieces([
            new PieceInput { Name = "MyPiece" },
            new PieceInput { Name = "" },
            new PieceInput { Name = "" },
        ]);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count);
        Assert.Equal("MyPiece", result.Value[0].Name);
        Assert.Equal("MyPiece#2", result.Value[1].Name);
        Assert.Equal("MyPiece#3", result.Value[2].Name);
    }

    [Fact]
    public void BuildPieces_SubsequentPiecesWithName_KeepsName()
    {
        Result<List<Piece>> result = BuildPieces([
            new PieceInput { Name = "Primera" },
            new PieceInput { Name = "Segunda" },
        ]);

        Assert.True(result.IsSuccess);
        Assert.Equal("Primera", result.Value[0].Name);
        Assert.Equal("Segunda", result.Value[1].Name);
    }

    [Fact]
    public void BuildPieces_NullFirstName_ReturnsFail()
    {
        Result<List<Piece>> result = BuildPieces([new PieceInput { Name = null }]);

        Assert.True(result.IsFailed);
    }
}

using Domain.Common;
using FluentResults;

namespace Domain.Models.Campaign;

public class Piece : Entity
{
    internal Piece() { }

    public static Piece CreatePiece(string name)
    {
        return new Piece
        {
            Name = name
        };
    }

    public int PieceId { get; set; }

    public int CampaignServiceId { get; set; }

    public BaseCampaignService CampaignService { get; set; } = null!;

    public string Name { get; set; } = null!;
}

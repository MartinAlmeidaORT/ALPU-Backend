using Domain.Common;

namespace Domain.Models.Campaign;

public class Piece : Entity
{
    public int PieceId { get; set; }

    public int CampaignServiceId { get; set; }

    public BaseCampaignService CampaignService { get; set; } = null!;

    public string Name { get; set; } = null!;
}

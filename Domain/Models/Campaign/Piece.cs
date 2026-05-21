using Domain.Common;

namespace Domain.Models.Campaign;

public class Piece : Entity
{
    public int CampaignServiceId { get; set; }

    public string Name { get; set; } = null!;
}

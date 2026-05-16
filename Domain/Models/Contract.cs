using Domain.Common;
using Domain.Models.Campaign;

namespace Domain.Models;

public class Contract : Entity
{
    public int ContractId { get; set; }

    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public int BroadcasterId { get; set; }

    public virtual Broadcaster Broadcaster { get; set; } = null!;

    public DateOnly Date { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal TotalPrice { get; set; }

    public int TermYears { get; set; }

    public virtual ICollection<Campaign.Campaign> Campaigns { get; set; } = [];

    public virtual ICollection<Bill> Bills { get; set; } = [];

    public string CountryCode { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;
}

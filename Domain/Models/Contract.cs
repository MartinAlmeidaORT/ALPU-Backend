using Domain.Common;

namespace Domain.Models;

public partial class Contract : Entity
{
    public int ContractId { get; set; }

    public int ClientId { get; set; }

    public int BroadcasterId { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly DueDate { get; set; }

    public string CountryCode { get; set; } = null!;

    public decimal Price { get; set; }

    public int? DiscountId { get; set; }

    public int TermYears { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = [];

    public virtual Broadcaster Broadcaster { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual Country CountryCodeNavigation { get; set; } = null!;

    public virtual ICollection<Piece> Pieces { get; set; } = [];

    public virtual ICollection<Discount> Discounts { get; set; } = [];
}

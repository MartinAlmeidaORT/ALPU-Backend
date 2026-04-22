using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("broadcaster")]
public partial class Broadcaster : User
{
    [Column("category_id")]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Broadcasters")]
    public virtual BroadcasterCategory Category { get; set; } = null!;

    [InverseProperty("Broadcaster")]
    public virtual ICollection<Contract> Contracts { get; set; } = [];

    [InverseProperty("Broadcaster")]
    public virtual ICollection<Demo> Demos { get; set; } = [];

    [InverseProperty("Broadcaster")]
    public virtual ICollection<Membership> Memberships { get; set; } = [];
}

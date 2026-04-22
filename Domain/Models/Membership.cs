using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;
using Domain.Enums;

namespace Domain.Models;

[Table("membership")]
public partial class Membership : Entity
{
    [Key]
    [Column("membership_id")]
    public int MembershipId { get; set; }

    [Column("state", TypeName = "membership_state_enum")]
    [EnumDataType(typeof(MembershipState))]
    public MembershipState State { get; set; }

    [Column("broadcaster_id")]
    public int BroadcasterId { get; set; }

    [Column("pay_date")]
    public DateOnly? PayDate { get; set; }

    [Column("due_date")]
    public DateOnly? DueDate { get; set; }

    [Column("amount")]
    public decimal? Amount { get; set; }

    [ForeignKey("BroadcasterId")]
    [InverseProperty("Memberships")]
    public virtual Broadcaster Broadcaster { get; set; } = null!;
}

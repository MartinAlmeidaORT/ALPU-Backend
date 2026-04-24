using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Inputs.Auth;

namespace Domain.Models;

[Table("broadcaster")]
public partial class Broadcaster : User
{
    public Broadcaster() { }

    public Broadcaster(RegisterBroadcasterInput input, Country country, BroadcasterCategory category)
        : base(input, country)
    {
        CategoryId = category.BroadcasterCategoryId;
        Category = category;
    }

    public Broadcaster(CompleteGoogleSignUpBroadcasterInput input, Country country, BroadcasterCategory category)
        : base(input, country)
    {
        CategoryId = category.BroadcasterCategoryId;
        Category = category;
    }

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

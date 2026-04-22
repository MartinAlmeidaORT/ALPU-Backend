using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Classes;

namespace Domain.Models;

[Table("broadcaster_category")]
public partial class BroadcasterCategory : Entity
{
    [Key]
    [Column("broadcaster_category_id")]
    public int BroadcasterCategoryId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("lifetime_job_count")]
    public int LifetimeJobCount { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Broadcaster> Broadcasters { get; set; } = [];
}

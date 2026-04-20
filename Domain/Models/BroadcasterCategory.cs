using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("broadcaster_category")]
public partial class BroadcasterCategory
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

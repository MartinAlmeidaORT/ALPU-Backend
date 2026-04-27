using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Models;

[Table("region")]
public partial class Region : Entity
{
    [Key]
    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("multiplier")]
    public decimal Multiplier { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Country> Countries { get; set; } = [];
}

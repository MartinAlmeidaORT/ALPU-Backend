using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Models;

[Table("country")]
public partial class Country : Entity
{
    [Key]
    [Column("country_code")]
    [StringLength(3)]
    public string CountryCode { get; set; } = null!;

    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("CountryCodeNavigation")]
    public virtual ICollection<Contract> Contracts { get; set; } = [];

    [ForeignKey("RegionId")]
    [InverseProperty("Countries")]
    public virtual Region? Region { get; set; }
}

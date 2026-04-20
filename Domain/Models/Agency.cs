using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("agency")]
public partial class Agency(string name)
{
    [Key]
    [Column("agency_id")]
    public int AgencyId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = name;

    [InverseProperty("Agency")]
    public virtual ICollection<Client> Clients { get; set; } = [];
}

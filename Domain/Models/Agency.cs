using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Models;

[Table("agency")]
public partial class Agency : Entity
{
#pragma warning disable CS8618
    [Obsolete("EF Core only", error: false)]
    public Agency() { }
#pragma warning restore CS0618

    public Agency(string name)
    {
        Name = name;
    }

    [Key]
    [Column("agency_id")]
    public int AgencyId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [InverseProperty("Agency")]
    public virtual ICollection<Client> Clients { get; set; } = [];
}

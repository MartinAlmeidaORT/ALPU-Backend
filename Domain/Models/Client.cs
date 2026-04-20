using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("client")]
public partial class Client : User
{
    [Column("agency_id")]
    public int AgencyId { get; set; }

    [ForeignKey("AgencyId")]
    [InverseProperty("Clients")]
    public virtual Agency Agency { get; set; } = null!;

    [InverseProperty("Client")]
    public virtual ICollection<Contract> Contracts { get; set; } = [];
}

using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Inputs.Auth;

namespace Domain.Models;

[Table("client")]
public partial class Client : User
{
    public Client() { }

    public Client(RegisterClientInput input, Country country, Agency agency)
        : base(input, country)
    {
        AgencyId = agency.AgencyId;
        Agency = agency;
    }

    public Client(CompleteGoogleSignUpClientInput input, Country country, Agency agency)
        : base(input, country)
    {
        AgencyId = agency.AgencyId;
        Agency = agency;
    }

    [Column("agency_id")]
    public int AgencyId { get; set; }

    [ForeignKey("AgencyId")]
    [InverseProperty("Clients")]
    public virtual Agency Agency { get; set; } = null!;

    [InverseProperty("Client")]
    public virtual ICollection<Contract> Contracts { get; set; } = [];
}

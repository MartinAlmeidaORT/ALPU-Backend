using Domain.Common;

namespace Domain.Models;

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

    public int AgencyId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = [];
}

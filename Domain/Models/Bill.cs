using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public partial class Bill : Entity
{
    public int BillId { get; set; }

    public BillType State { get; set; }

    public int? ContractId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string ProofFile { get; set; } = null!;

    public virtual Contract? Contract { get; set; }
}

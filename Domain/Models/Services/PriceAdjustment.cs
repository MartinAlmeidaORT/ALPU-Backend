using Domain.Common;
using Domain.Enums;

namespace Domain.Models.Services;

public class PriceAdjustment : Entity
{
    internal PriceAdjustment() { }

    public int PriceAdjustmentId { get; set; }

    public string Name { get; set; } = null!;

    public PriceAdjustmentType Type { get; set; }

    public decimal Amount { get; set; }

    public string Key { get; set; } = null!;
}

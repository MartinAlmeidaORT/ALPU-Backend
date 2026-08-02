using Domain.Common.Abstracts;
using Domain.Enums;

namespace Domain.Models.Services;

public class MultiServiceDiscount : Entity
{
    internal MultiServiceDiscount() { }

    public string Name { get; set; } = null!;

    public string Key { get; set; } = null!;

    public ServiceType ServiceA { get; set; }

    public ServiceType ServiceB { get; set; }

    public PriceAdjustmentType Type { get; set; }

    public decimal Amount { get; set; }

    public bool IsDiscountForServiceBOnly { get; set; }
}

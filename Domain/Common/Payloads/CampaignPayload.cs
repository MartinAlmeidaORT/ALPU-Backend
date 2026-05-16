using Domain.Enums;

namespace Domain.Common.Payloads;

public record PriceBreakdown
{
    public List<ServiceBreakdown> Services { get; init; } = [];
    public List<PriceAdjustmentBreakdown> Adjustments { get; set; } = [];
    public decimal Total { get; set; }
}

public record PriceAdjustmentBreakdown
{
    public PriceAdjustmentBreakdown(string key, decimal amount, decimal applyDiscount, PriceAdjustmentType type)
    {
        Key = key;
        Amount = amount;
        ApplyDiscount = applyDiscount;
        Type = type;
    }

    public string Key { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal ApplyDiscount { get; set; }
    public PriceAdjustmentType Type { get; set; }
}

public record ServiceBreakdown
{
    public string ServiceName { get; init; } = null!;
    public ServiceType ServiceType { get; init; }
    public decimal BasePrice { get; set; }
    public decimal? SubsequentPrice { get; init; }
    public decimal? VolumeDiscount { get; init; }
    public decimal SubTotal { get; set; }
    public List<PriceAdjustmentBreakdown> Adjustments { get; set; } = [];
    public PieceBreakdown[] Pieces { get; init; } = null!;
}

public record PieceBreakdown
{
    public PieceBreakdown()
    {

    }

    public PieceBreakdown(string name, bool isSubsequent, decimal price)
    {
        Name = name;
        IsSubsequent = isSubsequent;
        Price = price;
    }

    public string Name { get; init; } = null!;
    public bool IsSubsequent { get; init; }
    public decimal Price { get; init; }
}

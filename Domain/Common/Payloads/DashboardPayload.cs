using Domain.Models;

namespace Domain.Common.Payloads;

public record DashboardPayload
{
    public required decimal TotalIncome { get; init; }

    public required decimal TotalExpense { get; init; }

    public required UserPayload[] TopClientsByContracts { get; init; }

    public required UserPayload[] TopBroadcasterByContracts { get; init; }
}

public record UserPayload
{
    public required User User { get; init; }

    public required int Contracts { get; init; }
}

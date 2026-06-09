using Domain.Models;

namespace Domain.Common.Payloads;

public record DashboardPayload
{
    public required decimal TotalIncome { get; init; }

    public required decimal TotalExpense { get; init; }

    public required UserPayload[] TopClientsByContracts { get; init; }

    public required UserPayload[] TopBroadcasterByContracts { get; init; }

    public required PaidContractsPayload[] TopClientsByPaidContracts { get; init; }

    public required MonthlyTrendGroup[] MonthlyPaidGroup { get; init; }

    public required MonthlyDelinquentGroup[] MonthlyDelinquentsGroup { get; init; }
}

public record UserPayload
{
    public required User User { get; init; }

    public required int Contracts { get; init; }
}

public record MonthlyTrendGroup
{
    public required string Month { get; init; } // "2026-04", "2026-05"
    public required PaidContractsPayload[] Clients { get; init; }
}

public record PaidContractsPayload
{
    public required int ClientId { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required int PaidContracts { get; init; }

    public required int Month { get; init; }

    public required int Year { get; init; }
}

public record MonthlyDelinquentGroup
{
    public required string Month { get; init; }
    public required DelinquentClientPayload[] Clients { get; init; }
}

public record DelinquentClientPayload
{
    public required int ClientId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required int LateContracts { get; init; }
}

using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public IUnitOfWork _unitOfWork = unitOfWork;

    private readonly int _amountToTake = 20;

    public async Task<DashboardPayload> GetDashboardDataAsync()
    {
        DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly calendarCutoff = _today.AddMonths(-12);

        var totalIncomeTask = await GetTotalIncome().SumAsync();
        var totalExpenseTask = await GetTotalExpense().SumAsync();
        var topClientsByContractsTask = await GetTopClientsByContracts().ToArrayAsync();
        var topBroadcasterByContractsTask = await GetTopBroadcastersByContracts().ToArrayAsync();
        var topClientsByPaidContractsTask = await GetTopClientsByPaidContracts(calendarCutoff).ToArrayAsync();
        var monthlyDelinquentTrend = await GetMonthlyDelinquentClientsAsync(calendarCutoff);

        var monthlyTopClientsTrend = topClientsByPaidContractsTask
            .GroupBy(x => $"{x.Month:D2}-{x.Year}")
            .OrderBy(monthGroup => monthGroup.Key)
            .Select(monthGroup => new MonthlyTrendGroup
            {
                Month = monthGroup.Key,
                Clients = monthGroup
                    .Select(x => new PaidContractsPayload
                    {
                        ClientId = x.ClientId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        PaidContracts = x.PaidContracts,
                        Year = x.Year,
                        Month = x.Month
                    })
                    .OrderByDescending(c => c.PaidContracts)
                    .ToArray()
            })
            .ToArray();

        return new DashboardPayload()
        {
            TotalIncome = totalIncomeTask,
            TotalExpense = totalExpenseTask,
            TopClientsByContracts = topClientsByContractsTask,
            TopBroadcasterByContracts = topBroadcasterByContractsTask,
            TopClientsByPaidContracts = topClientsByPaidContractsTask,
            MonthlyPaidGroup = monthlyTopClientsTrend,
            MonthlyDelinquentsGroup = monthlyDelinquentTrend
        };
    }

    private IQueryable<decimal> GetTotalIncome() => _unitOfWork.Bills.GetAllBills().Where(b => b.Type == BillType.Income).Select(b => b.Amount);

    private IQueryable<decimal> GetTotalExpense() => _unitOfWork.Bills.GetAllBills().Where(b => b.Type == BillType.Expense).Select(b => b.Amount);

    private IQueryable<UserPayload> GetTopClientsByContracts()
    {
        return _unitOfWork.Clients.GetAllClients()
            .OrderByDescending(c => c.Contracts.Count)
            .Take(_amountToTake)
            .Select(c => new UserPayload
            {
                User = c,
                Contracts = c.Contracts.Count
            });
    }

    private IQueryable<UserPayload> GetTopBroadcastersByContracts()
    {
        return _unitOfWork.Broadcasters.GetAllBroadcasters()
            .OrderByDescending(b => b.Contracts.Count)
            .Take(_amountToTake)
            .Select(b => new UserPayload
            {
                User = b,
                Contracts = b.Contracts.Count
            });
    }

    private IQueryable<PaidContractsPayload> GetTopClientsByPaidContracts(DateOnly? calendarCutoff)
    {
        return _unitOfWork.Clients.GetAllClients()
            .SelectMany(client => client.Contracts.Select(contract => new
            {
                User = client,
                Contract = contract,
                LastPaymentDate = contract.Bills
                    .Where(b => b.Type == BillType.Income)
                    .Max(b => (DateOnly?)b.Date),
                IsPaid = contract.Bills
                    .Where(b => b.Type == BillType.Income)
                    .Sum(b => b.Amount) >= contract.TotalPrice
            }))
            .Where(x => x.IsPaid && x.LastPaymentDate >= calendarCutoff)
            .GroupBy(x => new
            {
                x.User.UserId,
                x.User.FirstName,
                x.User.LastName,
                x.User.Email,
                x.LastPaymentDate!.Value.Year,
                x.LastPaymentDate!.Value.Month
            })
            .Select(group => new PaidContractsPayload
            {
                ClientId = group.Key.UserId,
                FirstName = group.Key.FirstName,
                LastName = group.Key.LastName,
                Email = group.Key.Email,
                PaidContracts = group.Count(),
                Month = group.Key.Month,
                Year = group.Key.Year
            });
    }

    private async Task<MonthlyDelinquentGroup[]> GetMonthlyDelinquentClientsAsync(DateOnly calendarCutoff)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var monthsTimeline = Enumerable.Range(0, 12)
            .Select(i => today.AddMonths(-i))
            .Select(d => new { d.Month, d.Year })
            .ToList();

        var activeClientsData = await _unitOfWork.Clients.GetAllClients()
            .Where(client => client.Contracts.Any())
            .Select(client => new
            {
                client.UserId,
                client.FirstName,
                client.LastName,
                client.Email,
                Contracts = client.Contracts.Select(contract => new
                {
                    contract.ContractId,
                    contract.Date,
                    IncomeBills = contract.Bills
                        .Where(b => b.Type == BillType.Income)
                        .Select(b => new { b.Date.Month, b.Date.Year })
                })
            })
            .ToArrayAsync();

        var delinquentGroups = monthsTimeline
            .Select(time =>
            {
                var stringMonth = $"{time.Month:D2}-{time.Year}";
                var endOfTargetMonth = new DateOnly(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month));

                var lateClients = activeClientsData
                    .Select(client =>
                    {
                        var activeContracts = client.Contracts.Where(c =>
                            c.Date <= endOfTargetMonth).ToList();

                        int lateContractsCount = 0;

                        foreach (var contract in activeContracts)
                        {
                            var lastPaymentBeforeThisMonth = contract.IncomeBills
                                .Where(b => new DateOnly(b.Year, b.Month, 1) <= endOfTargetMonth)
                                .Select(b => (DateOnly?)new DateOnly(b.Year, b.Month, 1))
                                .Max();

                            var baseTrackingDate = lastPaymentBeforeThisMonth ?? contract.Date;

                            int monthsPassed = ((endOfTargetMonth.Year - baseTrackingDate.Year) * 12) +
                                                (endOfTargetMonth.Month - baseTrackingDate.Month);

                            if (monthsPassed > 1)
                            {
                                lateContractsCount++;
                            }
                        }

                        return new
                        {
                            Client = client,
                            LateCount = lateContractsCount
                        };
                    })
                    .Where(x => x.LateCount > 0)
                    .Select(x => new DelinquentClientPayload
                    {
                        ClientId = x.Client.UserId,
                        FirstName = x.Client.FirstName,
                        LastName = x.Client.LastName,
                        Email = x.Client.Email,
                        LateContracts = x.LateCount
                    })
                    .OrderByDescending(c => c.LateContracts)
                    .ToArray();

                return new MonthlyDelinquentGroup
                {
                    Month = stringMonth,
                    Clients = lateClients
                };
            })
            .OrderBy(g => g.Month)
            .ToArray();

        return delinquentGroups;
    }
}

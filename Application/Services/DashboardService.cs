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
            MonthlyPaidGroup = monthlyTopClientsTrend
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
}

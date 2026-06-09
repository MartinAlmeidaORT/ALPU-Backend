using Domain.Common.Payloads;
using Domain.Enums;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<DashboardPayload> GetDashboardDataAsync()
    {
        const int AMOUNT_TO_TAKE = 20;
        return new DashboardPayload()
        {
            TotalIncome = await _unitOfWork.Bills.GetAllBills().Where(b => b.Type == BillType.Income).SumAsync(b => b.Amount),
            TotalExpense = await _unitOfWork.Bills.GetAllBills().Where(b => b.Type == BillType.Expense).SumAsync(b => b.Amount),
            TopClientsByContracts = await _unitOfWork.Clients.GetAllClients()
                    .OrderByDescending(c => c.Contracts.Count)
                    .Take(AMOUNT_TO_TAKE)
                    .Select(c => new UserPayload
                    {
                        User = c,
                        Contracts = c.Contracts.Count
                    })
                    .ToArrayAsync(),
            TopBroadcasterByContracts = await _unitOfWork.Broadcasters.GetAllBroadcasters()
                .OrderByDescending(b => b.Contracts.Count)
                .Take(AMOUNT_TO_TAKE)
                .Select(b => new UserPayload
                {
                    User = b,
                    Contracts = b.Contracts.Count
                })
                .ToArrayAsync(),
            TopClientsByPaidContracts = await _unitOfWork.Clients.GetAllClients()
                .Select(client => new
                {
                    Client = client,
                    // Count how many contracts for this client are fully paid
                    PaidContractsCount = client.Contracts.Count(contract =>
                        contract.Bills
                            .Where(bill => bill.Type == BillType.Income)
                            .Sum(bill => bill.Amount) >= contract.TotalPrice)
                })
                // Filter out clients who don't have any fully paid contracts (optional)
                .Where(x => x.PaidContractsCount > 0)
                // Order by the count of paid contracts
                .OrderByDescending(x => x.PaidContractsCount)
                .Take(AMOUNT_TO_TAKE)
                // Project into your final payload
                .Select(x => new UserPayload
                {
                    User = x.Client,
                    Contracts = x.PaidContractsCount
                })
                .ToArrayAsync()
        };
    }
}

using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IBillRepository
{
    public Bill CreateBill(Bill entity);

    public IQueryable<Bill> GetAllBills();

    public Task<Bill?> GetBillByIdAsync(int id);

    public void DeleteBill(Bill entity);
}

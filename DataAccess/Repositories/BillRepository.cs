using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class BillRepository(DatabaseContext context) : RepositoryBase<Bill>(context), IBillRepository
{
    public Bill CreateBill(Bill entity) => Create(entity);

    public IQueryable<Bill> GetAllBills() => GetAll();

    public async Task<Bill?> GetBillByIdAsync(int id) => await Get(id);

    public void DeleteBill(Bill entity) => Delete(entity);
}

using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class BillRepository(DatabaseContext context) : RepositoryBase<Bill>(context), IBillRepository
{
    public Bill CreateBill(Bill entity) => Create(entity);

    public IQueryable<Bill> GetAllBills() => GetAll();

    public Bill DeleteBill(Bill entity) => Delete(entity);
}

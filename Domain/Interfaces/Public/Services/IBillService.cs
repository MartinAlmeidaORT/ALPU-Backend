using Domain.Common.Inputs;
using Domain.Models;
using FluentResults;

namespace Domain.Interfaces.Public.Services;

public interface IBillService
{
    Task<Result<Bill>> RegisterBillAsync(BillInput input);

    IQueryable<Bill> GetAllBills();
}

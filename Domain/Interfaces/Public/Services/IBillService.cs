using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Models;
using FluentResults;

namespace Domain.Interfaces.Public.Services;

public interface IBillService
{
    Task<Result<RegisterBillPayload>> RegisterBillAsync(BillInput input);

    IQueryable<Bill> GetAllBills();

    string GetBillProofDownloadUrl(Bill bill);
}

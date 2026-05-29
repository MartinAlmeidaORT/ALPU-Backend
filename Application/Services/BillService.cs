using Domain.Common.Inputs;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models;
using FluentResults;

namespace Application.Services;

public class BillService(IUnitOfWork unitOfWork) : IBillService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<Bill> GetAllBills()
    {
        return _unitOfWork.Bills.GetAllBills();
    }

    public async Task<Result<Bill>> RegisterBillAsync(BillInput input)
    {
        Contract? contract = await _unitOfWork.Contracts.GetByIdAsync(input.ContractId);

        if (contract == null)
        {
            return Result.Fail(ContractErrors.ContractNotFound(input.ContractId));
        }



        Bill newBill = Bill.CreateBill(input.Type, input.Title, input.Description, input.Date, input.Amount, contract, );

        _unitOfWork.Bills.CreateBill();
    }
}

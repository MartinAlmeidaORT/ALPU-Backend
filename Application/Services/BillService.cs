using DataAccess.ExternalServices;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models;
using FluentResults;

namespace Application.Services;

public class BillService(IUnitOfWork unitOfWork, AmazonS3Service amazonS3Service) : IBillService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AmazonS3Service _amazonS3Service = amazonS3Service;

    public IQueryable<Bill> GetAllBills()
    {
        return _unitOfWork.Bills.GetAllBills();
    }

    public async Task<Result<RegisterBillPayload>> RegisterBillAsync(BillInput input)
    {
        Contract? contract = null;
        if (input.ContractId != null)
        {
            contract = await _unitOfWork.Contracts.GetByIdAsync((int)input.ContractId);

            if (contract == null)
            {
                return Result.Fail(ContractErrors.ContractNotFound((int)input.ContractId));
            }
        }

        Result<Bill> newBill = Bill.CreateBill(input.Type, input.Title, input.Description, input.Date, input.Amount, contract);
        var amazonS3 = await _amazonS3Service.SaveBillProofAsync(input.FileName, input.Type);

        if (newBill.IsSuccess && amazonS3.IsSuccess)
        {
            newBill.Value.ProofFile = amazonS3.Value.Item1;
            _unitOfWork.Bills.CreateBill(newBill.Value);
            await _unitOfWork.SaveChangesAsync();
        }

        return new RegisterBillPayload()
        {
            Bill = newBill.Value,
            AmazonS3Url = amazonS3.Value.Item2
        };
    }

    public string GetBillProofDownloadUrl(Bill bill)
    {
        return _amazonS3Service.GetDownloadUrl(bill.ProofFile);
    }

    public async Task<Result<Bill>> DeleteBillAsync(int billId)
    {
        Bill? bill = await _unitOfWork.Bills.GetBillByIdAsync(billId);
        if (bill == null)
        {
            return Result.Fail(BillErrors.BillNotFound(billId));
        }

        _unitOfWork.Bills.DeleteBill(bill);
        _unitOfWork.SaveChangesAsync();
        return bill;
    }
}

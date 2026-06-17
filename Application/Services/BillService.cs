using DataAccess.ExternalServices;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class BillService(IUnitOfWork unitOfWork, AmazonS3Service amazonS3Service, IUserService userService) : IBillService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AmazonS3Service _amazonS3Service = amazonS3Service;
    private readonly IUserService _userService = userService;

    public IQueryable<Bill> GetAllBills()
    {
        return _unitOfWork.Bills.GetAllBills();
    }

    public async Task<Result<RegisterBillPayload>> RegisterBillAsync(BillInput input)
    {
        Contract? contract = null;
        if (input.ContractId != null)
        {
            contract = _unitOfWork.Contracts.GetAllContracts()
                .Include(c => c.Client)
                .Include(c => c.Broadcaster)
                .SingleOrDefault(c => c.ContractId == (int)input.ContractId);

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
            if (newBill.Value.Contract != null)
            {
                await _userService.AddNotificationAsync(
                    newBill.Value.Contract.Client,
                    $"Pago del contrato: {newBill.Value.Contract.ContractId}",
                    $"Se registro el pago con la suma de {newBill.Value.Amount}."
                );
                await _userService.AddNotificationAsync(
                    newBill.Value.Contract.Broadcaster,
                    $"Pago del contrato: {newBill.Value.Contract.ContractId}",
                    $"Cliente {newBill.Value.Contract.Client.FullName} pago la suma de {newBill.Value.Amount}."
                );
            }
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
        await _unitOfWork.SaveChangesAsync();
        return bill;
    }
}

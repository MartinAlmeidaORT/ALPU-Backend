using DataAccess.ExternalServices;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Enums;
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
        if (input.ContractSerial != null)
        {
            contract = _unitOfWork.Contracts.GetAllContracts()
                .Include(c => c.Client)
                .Include(c => c.Broadcaster)
                .Include(c => c.Bills)
                .SingleOrDefault(c => c.ContractSerial == input.ContractSerial);

            if (contract == null)
            {
                return Result.Fail(ContractErrors.ContractNotFound(input.ContractSerial));
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
                if (newBill.Value.Contract.State == ContractState.Pending || newBill.Value.Contract.State == ContractState.Canceled)
                {
                    return Result.Fail(ContractErrors.ContractNotActive(newBill.Value.Contract.ContractId));
                }
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
                decimal totalAmount = newBill.Value.Contract.Bills.Sum(b => b.Amount);
                if (totalAmount >= newBill.Value.Contract.TotalPricePostTax)
                {
                    await _userService.AddNotificationAsync(
                        newBill.Value.Contract.Client,
                        $"Contrato {newBill.Value.Contract.ContractId} completado",
                        $"Se registro el pago final del contrato."
                    );
                    await _userService.AddNotificationAsync(
                        newBill.Value.Contract.Broadcaster,
                        $"Contrato {newBill.Value.Contract.ContractId} completado",
                        $"Cliente {newBill.Value.Contract.Client.FullName} completo el pago del contrato."
                    );
                    newBill.Value.Contract.State = ContractState.Paid;
                }
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

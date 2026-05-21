using Application.Interfaces.Public.Services;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using FluentResults;

namespace Application.Services;

public class ContractService(IUnitOfWork unitOfWork) : IContractService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CalculateContractPayload>> CalculateContract(CalculateContractInput input)
    {
        throw new NotImplementedException();
    }

    public Task<Contract> CreateContractAsync(Contract contract)
    {
        throw new NotImplementedException();
    }

    public Task<Contract> DeleteContractAsync(int id)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Contract> GetAllContracts()
    {
        return _unitOfWork.Contracts.GetAllContracts();
    }

    public IQueryable<Contract> GetAllContracts(int userId, string role)
    {
        return role switch
        {
            "Administrator" or "Supervisor" or "Accountant" => _unitOfWork.Contracts.GetAllContracts(),
            "Client" => _unitOfWork.Contracts.GetAllContracts().Where(c => c.ClientId == userId),
            "Broadcaster" => _unitOfWork.Contracts.GetAllContracts().Where(c => c.BroadcasterId == userId),
            _ => Enumerable.Empty<Contract>().AsQueryable()
        };
    }

    public Task<Contract?> GetContractByIdAsync(int id)
    {
        return _unitOfWork.Contracts.GetByIdAsync(id);
    }

    public async Task<Result> UpdateContractAsync(UpdateContractStateInput input, int userId)
    {
        Contract? contract = _unitOfWork.Contracts
            .GetAllContracts()
            .Where(c => (c.ClientId == userId || c.BroadcasterId == userId) && c.ContractId == input.ContractId)
            .SingleOrDefault();

        if (contract == null)
        {
            return Result.Fail($"El contrato con id: {input.ContractId} no existe o no tiene acceso al mismo.");
        }

        if (contract.State == input.NewState)
        {
            return Result.Fail("El contrato ya se encuentra en ese estado.");
        }

        contract.State = input.NewState;
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}

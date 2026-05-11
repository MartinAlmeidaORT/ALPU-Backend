using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Models;
using FluentResults;

namespace Application.Interfaces.Public.Services;

public interface IContractService
{
    Task<Result<CalculateContractPayload>> CalculateContract(CalculateContractInput input);
    IQueryable<Contract> GetAllContracts();
    Task<Contract?> GetContractByIdAsync(int id);
    Task<Contract> CreateContractAsync(Contract contract);
    Task<Contract> UpdateContractAsync(Contract contract);
    Task<Contract> DeleteContractAsync(int id);
}

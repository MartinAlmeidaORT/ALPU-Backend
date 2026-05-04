using Application.Common;
using Application.Interfaces.Public.Services;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace Application.Services;

public class ContractService(IUnitOfWork unitOfWork) : IContractService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ResultAPI<CalculateContractPayload>> CalculateContract(CalculateContractInput input)
    {
        List<Service> services = [];
        foreach (var service in input.Services)
        {
            Service? serviceResult = await _unitOfWork.Services.GetServiceByIdAsync(service.ServiceId);
            if (serviceResult is not null) services.Add(serviceResult);
        }

        // Checkear descuentos TV+radio
        decimal totalPrice = 0;
        for (int i = 0; i < services.Count; i++)
        {
            totalPrice += services[i].GetTotalPrice(input.Services[i]).Value;
        }

        CalculateContractPayload payload = new()
        {
            TotalPrice = totalPrice
        };
        return ResultAPI<CalculateContractPayload>.Success(payload);
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

    public Task<Contract?> GetContractByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Contract> UpdateContractAsync(Contract contract)
    {
        throw new NotImplementedException();
    }
}

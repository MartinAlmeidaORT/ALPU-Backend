using Application.Interfaces.Public.Services;
using Domain.Common.Inputs;
using Domain.Common.Payloads;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Domain.Models.Services;
using FluentResults;

namespace Application.Services;

public class ContractService(IUnitOfWork unitOfWork) : IContractService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CalculateContractPayload>> CalculateContract(CalculateContractInput input)
    {
        throw new NotImplementedException();

        List<BaseService> services = [];
        foreach (var service in input.Services)
        {
            BaseService? serviceResult = await _unitOfWork.Services.GetServiceByIdAsync(service.ServiceId);
            if (serviceResult is not null) services.Add(serviceResult);
        }

        // Checkear descuentos TV+radio
        ServicePricePayload[] pricesPayload = new ServicePricePayload[services.Count];
        decimal totalPrice = 0;
        for (int i = 0; i < services.Count; i++)
        {
            // pricesPayload[i] = services[i].GetTotalPrice(input.Services[i]).Value;
            totalPrice += pricesPayload[i].TotalPriceWithDiscount;
        }

        CalculateContractPayload payload = new()
        {
            TotalPrice = totalPrice,
            // ServicePriceWithDiscount = prices
            ServicePrice = pricesPayload,
        };
        return payload;
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

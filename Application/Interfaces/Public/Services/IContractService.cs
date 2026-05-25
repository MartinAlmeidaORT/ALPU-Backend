using Domain.Common.Inputs;
using Domain.Common.Inputs.CampaignService;
using Domain.Models;
using FluentResults;

namespace Application.Interfaces.Public.Services;

public interface IContractService
{
    IQueryable<Contract> GetAllContracts();

    IQueryable<Contract> GetAllContracts(int userId, string role);

    Task<Contract?> GetContractByIdAsync(int id);

    Task<Result<Contract>> CreateContractAsync(CampaignInput input);

    Task<Result> UpdateContractAsync(UpdateContractStateInput input, int userId);

    Task<Contract> DeleteContractAsync(int id);
}

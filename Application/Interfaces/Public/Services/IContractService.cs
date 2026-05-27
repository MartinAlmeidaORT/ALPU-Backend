using Domain.Common.Inputs;
using Domain.Common.Inputs.CampaignService;
using Domain.Common.Payloads;
using Domain.Models;
using FluentResults;

namespace Application.Interfaces.Public.Services;

public interface IContractService
{
    IQueryable<Contract> GetAllContracts();

    IQueryable<Contract> GetAllContracts(int userId, string role);

    Task<Contract?> GetContractByIdAsync(int id);

    Task<Result<GenerateContractPayload>> CreateContractAsync(CampaignInput input);

    Task<Result<string>> GetContractPdfDownloadUrl(Contract contract);

    Task<Result> UpdateContractAsync(UpdateContractStateInput input, int userId);

    Task<Result> ApproveContractAsync(int userId, int contractId);

    Task<Contract> DeleteContractAsync(int id);
}

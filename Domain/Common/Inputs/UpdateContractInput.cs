using Domain.Enums;

namespace Domain.Common.Inputs;

public record UpdateContractStateInput
{
    public int ContractId { get; init; }
    public ContractState NewState { get; init; }
}

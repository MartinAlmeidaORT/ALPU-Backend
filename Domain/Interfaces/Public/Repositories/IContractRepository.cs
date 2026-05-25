using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IContractRepository
{
    public Contract CreateContract(Contract entity);

    public IQueryable<Contract> GetAllContracts();

    public Task<Contract?> GetByIdAsync(int id);
}

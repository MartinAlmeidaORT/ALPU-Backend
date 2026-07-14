using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class ContractRepository(DatabaseContext context) : RepositoryBase<Contract>(context), IContractRepository
{
    public Contract CreateContract(Contract entity) => Create(entity);

    public IQueryable<Contract> GetAllContracts() => GetAll();

    public async Task<Contract?> GetByIdAsync(int id) => await Get(id);

    public async Task<Contract> GetContractWithFullDetailsAsync(int id)
    {
        return await GetAll()
            .Where(c => c.ContractId == id)
            .Include(c => c.Client.Agency)
            .Include(c => c.Client.Address.Country)
            .Include(c => c.Client.Address.Department)
            .Include(c => c.Broadcaster.Address.Country)
            .Include(c => c.Broadcaster.Address.Department)
            .Include(c => c.Broadcaster.Contracts)
            .SingleAsync();
    }

    public async Task<int> CountByRootIdAsync(int rootContractId) =>
        await GetAll().CountAsync(c => c.RootContractId == rootContractId);
}

using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class ContractRepository(DatabaseContext context) : RepositoryBase<Contract>(context), IContractRepository
{
    public IQueryable<Contract> GetAllContracts() => GetAll();

    public async Task<Contract?> GetByIdAsync(int id) => await Get(id);
}

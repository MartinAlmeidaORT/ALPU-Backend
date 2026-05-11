using DataAccess.EntityFramework;
using Domain.Common.Abstracts;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;

namespace DataAccess.Repositories;

public class DepartmentRepository(DatabaseContext context) : RepositoryBase<Department>(context), IDepartmentRepository
{
    public IQueryable<Department> GetAllDepartments() => GetAll();

    public async Task<Department?> GetByCodeAsync(int id) => await Get(id);
}

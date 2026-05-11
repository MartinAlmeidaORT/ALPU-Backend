using Domain.Models;

namespace Domain.Interfaces.Public.Repositories;

public interface IDepartmentRepository
{
    public IQueryable<Department> GetAllDepartments();

    public Task<Department?> GetByCodeAsync(int id);
}

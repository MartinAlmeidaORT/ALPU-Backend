using Domain.Models;

namespace Domain.Interfaces.Public.Services;

public interface IDepartmentService
{
    IQueryable<Department> GetAllDepartments();
}

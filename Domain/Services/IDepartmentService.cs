using Domain.Models;

namespace Application.Interfaces.Public.Services;

public interface IDepartmentService
{
    IQueryable<Department> GetAllDepartments();
}

using Domain.Interfaces.Public.Repositories;
using Domain.Interfaces.Public.Services;
using Domain.Models;

namespace Application.Services;

public class DepartmentService(IUnitOfWork unitOfWork) : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IQueryable<Department> GetAllDepartments() => _unitOfWork.Departments.GetAllDepartments();
}

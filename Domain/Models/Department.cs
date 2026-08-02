using Domain.Common.Abstracts;
using Domain.Common.Errors;

namespace Domain.Models;

public class Department : Entity
{
    internal Department() { }

    public int DepartmentId { get; set; }

    public string CountryCode { get; set; } = null!;

    public Country Country { get; set; } = null!;

    public string Name { get; set; } = null!;
}

public static class DepartmentErrors
{
    public class DepartmentNotFoundError(string msg) : NotFoundError(msg);

    public static DepartmentNotFoundError DepartmentNotFound(int departmentId) => new($"Department with id {departmentId} was not found.");
}

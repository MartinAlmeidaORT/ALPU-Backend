using Domain.Common;

namespace Domain.Models;

public class Department : Entity
{
    public int DepartmentId { get; set; }

    public string CountryCode { get; set; } = null!;

    public Country Country { get; set; } = null!;

    public string Name { get; set; } = null!;
}

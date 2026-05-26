using Domain.Common;
using Domain.Common.Errors;

namespace Domain.Models;

public class Country : Entity
{
    internal Country() { }

    public string CountryCode { get; set; } = null!;

    public int? RegionId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = [];

    public virtual ICollection<Department> Departments { get; set; } = [];

    public virtual Region? Region { get; set; }
}

public static class CountryErrors
{
    public class CountryNotFoundError(string msg) : NotFoundError(msg);

    public static CountryNotFoundError CountryNotFound(string countryCode) => new($"Country with code {countryCode} was not found.");
}

using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using FluentResults;

namespace Domain.Models;

public class Address : Entity
{
    internal Address() { }

    public Address(Country country, Department department, string city, string? street)
    {
        CountryCode = country.CountryCode;
        Country = country;
        DepartmentId = department.DepartmentId;
        Department = department;
        City = city.Trim();
        Street = street;
    }

    public void Update(Country? country, Department? department, UpdateAddressInput? input)
    {
        CountryCode = country?.CountryCode ?? CountryCode;
        Country = country ?? Country;
        DepartmentId = input?.DepartmentId ?? DepartmentId;
        Department = department ?? Department;
        City = input?.City ?? City.Trim();
        Street = input?.Street ?? Street;
    }

    public Result ValidateAddress()
    {
        return Result.Merge(
            ValidateCity(),
            ValidateDepartment(),
            ValidateStreet()
        );
    }

    public Result ValidateCity()
    {
        if (City == null) return AddressErrors.CityIsRequired();

        Result errors = new();

        if (City.Length < 4) errors.WithError(AddressErrors.CityMinLength(City));
        if (City.Length > 50) errors.WithError(AddressErrors.CityMaxLength(City));
        if (!City.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))) errors.WithError(AddressErrors.CityIsLettersOnly(City));

        return errors.IsSuccess ? Result.Ok() : errors;
    }

    public Result ValidateDepartment()
    {
        return Department != null ? Result.Ok() : AddressErrors.StateIsRequired();
    }

    public Result ValidateStreet()
    {
        if (Street == null) return Result.Ok();

        Result errors = new();

        if (Street.Length < 4) errors.WithError(AddressErrors.StreetMinLength(Street));
        if (Street.Length > 50) errors.WithError(AddressErrors.StreetMaxLength(Street));

        return errors.IsSuccess ? Result.Ok() : errors;
    }

    public int AddressId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? Street { get; set; }

    public string City { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public override string ToString()
    {
        return $"{City}, {Department.Name} {Street}";
    }
}

public static class AddressErrors
{
    public class AddressNotFoundError(string msg) : NotFoundError(msg);

    public class CityIsRequiredError(string msg) : ValidationError(msg);
    public class CityMinLengthError(string msg) : ValidationError(msg);
    public class CityMaxLengthError(string msg) : ValidationError(msg);
    public class CityIsLettersOnlyError(string msg) : ValidationError(msg);

    public class DepartmentIsRequiredError(string msg) : ValidationError(msg);

    public class StreetIsRequiredError(string msg) : ValidationError(msg);
    public class StreetMinLengthError(string msg) : ValidationError(msg);
    public class StreetMaxLengthError(string msg) : ValidationError(msg);

    // Factories
    public static AddressNotFoundError AddressNotFound(int AddressId) => new($"Address with ID {AddressId} was not found.");

    public static CityIsRequiredError CityIsRequired() => new($"City is required.");
    public static CityMinLengthError CityMinLength(string city) => new($"City must be at least 3 characters long. {city}");
    public static CityMaxLengthError CityMaxLength(string city) => new($"City must be at most 50 characters long. {city}");
    public static CityIsLettersOnlyError CityIsLettersOnly(string city) => new($"City must contain only letters. {city}");

    public static DepartmentIsRequiredError StateIsRequired() => new($"Department is required.");

    public static StreetIsRequiredError StreetIsRequired() => new($"Street is required.");
    public static StreetMinLengthError StreetMinLength(string street) => new($"Street must be at least 3 characters long. {street}");
    public static StreetMaxLengthError StreetMaxLength(string street) => new($"Street must be at most 50 characters long. {street}");
}

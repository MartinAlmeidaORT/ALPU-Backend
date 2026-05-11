using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using FluentResults;

namespace Domain.Models;

public partial class Address : Entity
{
    public Address() { }

    public Address(Country country, string state, string city, string? street)
    {
        CountryCode = country.CountryCode;
        Country = country;
        State = state;
        City = city;
        Street = street;
    }

    public void Update(Country? country, UpdateAddressInput? input)
    {
        CountryCode = country?.CountryCode ?? CountryCode;
        Country = country ?? Country;
        State = input?.State ?? State;
        City = input?.City ?? City;
        Street = input?.Street ?? Street;
    }

    public Result ValidateAddress()
    {
        return Result.Merge(
            ValidateCity(),
            ValidateState(),
            ValidateStreet()
        );
    }

    public Result ValidateCity()
    {
        if (City == null) return AddressErrors.CityIsRequired();

        Result errors = new();

        if (City.Length < 4) errors.WithError(AddressErrors.CityMinLength(City));
        if (City.Length > 50) errors.WithError(AddressErrors.CityMaxLength(City));
        if (!City.All(char.IsLetter)) errors.WithError(AddressErrors.CityIsLettersOnly(City));

        return errors.IsSuccess ? Result.Ok() : errors;
    }

    public Result ValidateState()
    {
        if (State == null) return AddressErrors.StateIsRequired();

        Result errors = new();

        if (State.Length < 4) errors.WithError(AddressErrors.StateMinLength(State));
        if (State.Length > 50) errors.WithError(AddressErrors.StateMaxLength(State));
        if (!State.All(char.IsLetter)) errors.WithError(AddressErrors.StateIsLettersOnly(State));

        return errors.IsSuccess ? Result.Ok() : errors;
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

    public string State { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;
}

public static class AddressErrors
{
    public class AddressNotFoundError(string msg) : NotFoundError(msg);

    public class CityIsRequiredError(string msg) : ValidationError(msg);
    public class CityMinLengthError(string msg) : ValidationError(msg);
    public class CityMaxLengthError(string msg) : ValidationError(msg);
    public class CityIsLettersOnlyError(string msg) : ValidationError(msg);

    public class StateIsRequiredError(string msg) : ValidationError(msg);
    public class StateMinLengthError(string msg) : ValidationError(msg);
    public class StateMaxLengthError(string msg) : ValidationError(msg);
    public class StateIsLettersOnlyError(string msg) : ValidationError(msg);

    public class StreetIsRequiredError(string msg) : ValidationError(msg);
    public class StreetMinLengthError(string msg) : ValidationError(msg);
    public class StreetMaxLengthError(string msg) : ValidationError(msg);

    // Factories
    public static AddressNotFoundError AddressNotFound(int AddressId) => new($"Address with ID {AddressId} was not found.");

    public static CityIsRequiredError CityIsRequired() => new($"City is required.");
    public static CityMinLengthError CityMinLength(string city) => new($"City must be at least 3 characters long. {city}");
    public static CityMaxLengthError CityMaxLength(string city) => new($"City must be at most 50 characters long. {city}");
    public static CityIsLettersOnlyError CityIsLettersOnly(string city) => new($"City must contain only letters. {city}");

    public static StateIsRequiredError StateIsRequired() => new($"State is required.");
    public static StateMinLengthError StateMinLength(string state) => new($"State must be at least 3 characters long. {state}");
    public static StateMaxLengthError StateMaxLength(string state) => new($"State must be at most 50 characters long. {state}");
    public static StateIsLettersOnlyError StateIsLettersOnly(string state) => new($"State must contain only letters. {state}");

    public static StreetIsRequiredError StreetIsRequired() => new($"Street is required.");
    public static StreetMinLengthError StreetMinLength(string street) => new($"Street must be at least 3 characters long. {street}");
    public static StreetMaxLengthError StreetMaxLength(string street) => new($"Street must be at most 50 characters long. {street}");
}

using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;

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

    public Result<AppError> ValidateAddress()
    {
        return Result<AppError>.Combine(
            ValidateCity(),
            ValidateState(),
            ValidateStreet()
        );
    }

    public Result<AppError> ValidateCity()
    {
        if (City == null) return Result<AppError>.Failure(AppError.Validation("City is required"));

        return Result<AppError>.Combine(
            Require(City.Length >= 4, "City must be at least 4 characters long"),
            Require(City.Length <= 100, "City must be at most 100 characters long"),
            Require(City.All(char.IsLetter), "City must contain only letters")
        );
    }

    public Result<AppError> ValidateState()
    {
        if (State == null) return Result<AppError>.Failure(AppError.Validation("State is required"));

        return Result<AppError>.Combine(
            Require(State.Length >= 4, "State must be at least 4 characters long"),
            Require(State.Length <= 100, "State must be at most 100 characters long"),
            Require(State.All(char.IsLetter), "State must contain only letters")
        );
    }

    public Result<AppError> ValidateStreet()
    {
        if (Street == null) return Result<AppError>.Success();

        return Result<AppError>.Combine(
            Require(Street.Length >= 4, "Street must be at least 4 characters long"),
            Require(Street.Length <= 100, "Street must be at most 100 characters long")
        );
    }

    public int AddressId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? Street { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;
}

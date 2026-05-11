using Domain.Models;
using FluentAssertions;
using Tests.Helpers;

namespace Tests.Domain;

public class AddressTests
{
    private Address BuildAddress()
    {
        return new()
        {
            AddressId = 1,
            Country = DomainBuilders.ValidCountry(),
            CountryCode = DomainBuilders.ValidCountry().CountryCode,
            State = "Estado",
            City = "Ciudad",
            Street = "Some Street"
        };
    }

    [Fact]
    //Verificar que el estado tenga un minimo de 4 caracteres
    public void ValidateAddress_WhenStateIsNull_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.State = null!;

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.StateIsRequiredError>();
    }

    [Fact]
    //Verificar que el estado tenga un minimo de 4 caracteres
    public void ValidateAddress_WhenStateBelowMinimumLength_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.State = "a";

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.StateMinLengthError>();
    }

    [Fact]
    //Verificar que el estado tenga un maximo de 100 caracteres
    public void ValidateAddress_WhenStateAboveMaximunLength_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.State = new('a', 60);

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.StateMaxLengthError>();
    }

    [Theory]
    [InlineData("Buenos Aires123")]
    [InlineData("Buenos@Aires")]
    [InlineData("Buenos-Aires")]
    //Verificar que el estado no contenga caracteres no alfabéticos
    public void ValidateAddress_WhenStateContainsNonLetters_ReturnsFailed(string state)
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.State = state;

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.StateIsLettersOnlyError>();
    }

    [Fact]
    //Verificar que la ciudad tenga un minimo de 4 caracteres
    public void ValidateAddress_WhenCityIsNull_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.City = null!;

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.CityIsRequiredError>();
    }

    [Fact]
    //Verificar que la ciudad tenga un minimo de 4 caracteres
    public void ValidateAddress_WhenCityBelowMinimumLength_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.City = "w";

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.CityMinLengthError>();
    }

    [Fact]
    //verificar que la ciudad tenga un maximo de 60 caracteres
    public void ValidateAddress_WhenCityExceedsMaximumLength_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.City = new('a', 60);

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.CityMaxLengthError>();
    }

    [Theory]
    [InlineData("Buenos Aires123")]
    [InlineData("Buenos@Aires")]
    [InlineData("Buenos-Aires")]
    //Verificar que la ciudad no contenga caracteres no alfabéticos
    public void ValidateAddress_WhenCityContainsNonLetters_ReturnsFailed(string city)
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.City = city;

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.CityIsLettersOnlyError>();
    }

    [Fact]
    //Verificar que la calle tenga un minimo de 4 caracteres
    public void ValidateAddress_WhenStreetBelowMinimumLength_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.Street = "a";

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.StreetMinLengthError>();
    }

    [Fact]
    //Verificar que la calle tenga un maximo de 100 caracteres
    public void ValidateAddress_WhenStreetExceedsMaximumLength_ReturnsFailed()
    {
        // Arrange
        var _sut = BuildAddress();
        _sut.Street = new('a', 60);

        // Act
        var result = _sut.ValidateAddress();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AddressErrors.StreetMaxLengthError>();
    }
}

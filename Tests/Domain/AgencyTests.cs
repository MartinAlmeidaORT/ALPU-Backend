using Domain.Models;
using FluentAssertions;

namespace Tests.Domain;

public class AgencyTests
{
    [Fact]
    public void RegisterClientAsync_WhenAgencyNameIsNull_ReturnsFail()
    {
        // Arrange
        Agency _sut = new(null!);

        // Act
        var result = _sut.Validate();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AgencyErrors.NameIsRequiredError>();
    }

    // Verificar que el nombre de agencia tenga un minimo de 3 caracteres
    [Fact]
    public void RegisterClientAsync_WhenAgencyNameBelowMinimumLength_ReturnsFail()
    {
        // Arrange
        Agency _sut = new("AB");

        // Act
        var result = _sut.Validate();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AgencyErrors.NameMinLengthError>();
    }

    // Verificar que el nombre de agencia tenga un maximo de 100 caracteres
    [Fact]
    public void RegisterClientAsync_WhenAgencyNameExceedsMaximumLength_ReturnsFail()
    {
        // Arrange
        Agency _sut = new(new string('a', 101));

        // Act
        var result = _sut.Validate();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AgencyErrors.NameMaxLengthError>();
    }

    [Theory]
    [InlineData("Agency123")]
    [InlineData("Agency@")]
    // Verificar que el nombre de agencia no contenga caracteres no alfabéticos
    public void RegisterClientAsync_WhenAgencyNameContainsNonLetters_ReturnsFail(string agencyName)
    {
        // Arrange
        Agency _sut = new(agencyName);

        // Act
        var result = _sut.Validate();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<AgencyErrors.NameIsLettersOnlyError>();
    }
}

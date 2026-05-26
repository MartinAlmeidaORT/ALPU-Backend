using Domain.Models;
using FluentAssertions;
using Tests.Helpers;

namespace Tests.Domain;

public class UserTests
{
    private User BuildUser()
    {
        var result = Broadcaster.SignUp(
            InputBuilders.ValidBroadcasterInput(),
            DomainBuilders.ValidCountry(),
            DomainBuilders.ValidDepartment(),
            new BroadcasterCategory { BroadcasterCategoryId = 1 },
            "hashed"
        );
        return result.Value;
    }

    [Fact]
    public void ValidateEmail_WhenMissingAtSymbol_ReturnsFail()
    {
        var user = BuildUser();
        user.Email = "invalidemail.com";

        var result = user.ValidateEmail();

        result.IsSuccess.Should().BeFalse();
        result.HasError<UserErrors.EmailIsMissingAtCharacterError>();
    }

    //Verificar que el email tenga un minimo de 10 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailBelowMinimumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.Email = "a@b.c";

        // Act
        var result = user.ValidateEmail();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.EmailMinLengthError>();
    }
    //Verificar que el email tenga un maximo de 100 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailExceedsMaximumLength_ReturnsFail()
    {
        var user = BuildUser();
        user.Email = new string('a', 90) + "@example.com";

        // Act
        var result = user.ValidateEmail();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.EmailMaxLengthError>();
    }

    //Verificar que la contraseña tenga un minimo de 10 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenPasswordBelowMinimumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.Password = "short";

        // Act
        var result = user.ValidatePassword();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.PasswordMinLengthError>();
    }

    //Verificar que la contraseña tenga un maximo de 60 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenPasswordExceedsMaximumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.Password = new string('a', 61);

        // Act
        var result = user.ValidatePassword();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.PasswordMaxLengthError>();
    }

    [Fact]
    public async Task RegisterBroadcasterAsync_WhenFirstNameBelowMinimumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.FirstName = "Jo";

        // Act
        var result = user.ValidateFirstName();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.FirstNameMinLengthError>();
    }

    //Verificar que el nombre tenga un maximo de 50 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenFirstNameExceedsMaximumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.FirstName = new string('a', 51);

        // Act
        var result = user.ValidateFirstName();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.FirstNameMaxLengthError>();
    }

    [Theory]
    [InlineData("Jane123")]
    [InlineData("Jane@")]
    [InlineData("Jane ")]
    //Verificar que el nombre no contenga caracteres no alfabéticos
    public async Task RegisterBroadcasterAsync_WhenFirstNameContainsNonLetters_ReturnsFail(string firstName)
    {
        // Arrange
        var user = BuildUser();
        user.FirstName = firstName;

        // Act
        var result = user.ValidateFirstName();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.FirstNameLettersOnlyError>();
    }

    //Verificar que el apellido tenga un minimo de 3 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenLastNameBelowMinimumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.LastName = "Do";

        // Act
        var result = user.ValidateLastName();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.LastNameMinLengthError>();
    }
    //Verificar que el apellido tenga un maximo de 50 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenLastNameExceedsMaximumLength_ReturnsFail()
    {
        // Arrange
        var user = BuildUser();
        user.LastName = new string('a', 51);

        // Act
        var result = user.ValidateLastName();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.LastNameMaxLengthError>();
    }

    [Theory]
    [InlineData("Smith123")]
    [InlineData("Smith@")]
    [InlineData("Smith ")]
    //Verificar que el apellido no contenga caracteres no alfabéticos
    public async Task RegisterBroadcasterAsync_WhenLastNameContainsNonLetters_ReturnsFail(string lastName)
    {
        // Arrange
        var user = BuildUser();
        user.LastName = lastName;

        // Act
        var result = user.ValidateLastName();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.LastNameLettersOnlyError>();
    }

    [Theory]
    [InlineData("12345678901")] // 11 caracteres
    [InlineData("1234567890123")] // 13 caracteres
    [InlineData("")] // vacío
    //Verificar que el RUT tenga exactamente 12 caracteres
    public async Task RegisterBroadcasterAsync_WhenRutInvalidLength_ReturnsFail(string rut)
    {
        // Arrange
        var user = BuildUser();
        user.RUT = rut;

        // Act
        var result = user.ValidateRUT();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.RutIsInvalidError>();
    }
}

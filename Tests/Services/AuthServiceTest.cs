// Services/AuthServiceTests.cs
using Application.Services;
using Domain.Common.Inputs.Auth;
using Domain.Common;
using Domain.Interfaces.Private;
using Domain.Interfaces.Public.Repositories;
using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Net;
using Tests.Helpers;
using Microsoft.AspNetCore.Components.Forms;

namespace Tests.Services;

public class AuthServiceTests
{
    // --- Substitutes ---
    private readonly IHasher _hasher = Substitute.For<IHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IConfiguration _config = Substitute.For<IConfiguration>();
    private readonly IGoogleAuthService _googleAuthService = Substitute.For<IGoogleAuthService>();

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        // JWT config mínima para que GenerateJWT no explote
        _config["JWT:Secret"].Returns("super-secret-key-for-testing-purposes-only-32chars");

        _sut = new AuthService(_hasher, _unitOfWork, _config, _googleAuthService);
    }

    // ---------------------------------------------------------------
    // RegisterBroadcasterAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterBroadcasterAsync_WhenCountryNotFound_ReturnsNotFound()
    {
        // Arrange
        var input = DomainBuilders.BroadcasterInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns((Country?)null);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterBroadcasterAsync_WhenCategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var input = DomainBuilders.BroadcasterInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country());
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(1).Returns((BroadcasterCategory?)null);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterBroadcasterAsync_WithValidInput_SavesBroadcasterAndReturnsToken()
    {
        // Arrange
        var input = DomainBuilders.BroadcasterInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(1).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _hasher.Hash(input.Password).Returns("hashed-password");

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrEmpty();

        // Verify persistence
        _unitOfWork.Broadcasters.Received(1).CreateBroadcaster(Arg.Any<Broadcaster>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    #region Broadcaster Email Validation
    // Verificar que el email tenga un arroba
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailMissingAtSymbol_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "invalidemailexample.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    //Verificar que el email no esté repetido
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailDuplicate_ReturnsConflict()
    {
        // Arrange
        var input = DomainBuilders.BroadcasterInput();
        var existingBroadcaster = DomainBuilders.Broadcaster();
        existingBroadcaster.Email = input.Email;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(Arg.Any<int>()).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(existingBroadcaster);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    //Verificar que el email tenga un minimo de 10 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "a@b.c",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    //Verificar que el email tenga un maximo de 100 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = new string('a', 90) + "@example.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Broadcaster Password Validation
    //Verificar que la contraseña tenga un minimo de 10 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenPasswordBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "short1",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    //Verificar que la contraseña tenga un maximo de 60 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenPasswordExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = new string('a', 61),
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Broadcaster First Name Validation
    //Verificar que el nombre tenga un minimo de 3 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenFirstNameBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "Jo",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    //Verificar que el nombre tenga un maximo de 50 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenFirstNameExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = new string('a', 51),
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Jane123")]
    [InlineData("Jane@")]
    [InlineData("Jane ")]
    //Verificar que el nombre no contenga caracteres no alfabéticos
    public async Task RegisterBroadcasterAsync_WhenFirstNameContainsNonLetters_ReturnsBadRequest(string firstName)
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = firstName,
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Broadcaster Last Name Validation
    //Verificar que el apellido tenga un minimo de 3 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenLastNameBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "Do",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    //Verificar que el apellido tenga un maximo de 50 caracteres
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenLastNameExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = new string('a', 51),
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Smith123")]
    [InlineData("Smith@")]
    [InlineData("Smith ")]
    //Verificar que el apellido no contenga caracteres no alfabéticos
    public async Task RegisterBroadcasterAsync_WhenLastNameContainsNonLetters_ReturnsBadRequest(string lastName)
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = lastName,
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Broadcaster RUT Validation

    [Theory]
    [InlineData("12345678901")] // 11 caracteres
    [InlineData("1234567890123")] // 13 caracteres
    [InlineData("")] // vacío
    //Verificar que el RUT tenga exactamente 12 caracteres
    public async Task RegisterBroadcasterAsync_WhenRutInvalidLength_ReturnsBadRequest(string rut)
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = rut,
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que el rut no esté repetido
    public async Task RegisterBroadcasterAsync_WhenRutDuplicate_ReturnsConflict()
    {
        // Arrange
        var input = DomainBuilders.BroadcasterInput();
        var existingBroadcaster = DomainBuilders.Broadcaster();
        existingBroadcaster.RUT = input.RUT;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(Arg.Any<int>()).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _unitOfWork.Users.GetUserByRutAsync(input.RUT).Returns(existingBroadcaster);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region Broadcaster City Validation

    [Fact]
    //Verificar que la ciudad tenga un minimo de 4 caracteres
    public async Task RegisterBroadcasterAsync_WhenCityBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "NYC",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //verificar que la ciudad tenga un maximo de 100 caracteres
    public async Task RegisterBroadcasterAsync_WhenCityExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = new string('a', 101),
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Buenos Aires123")]
    [InlineData("Buenos@Aires")]
    [InlineData("Buenos-Aires")]
    //Verificar que la ciudad no contenga caracteres no alfabéticos
    public async Task RegisterBroadcasterAsync_WhenCityContainsNonLetters_ReturnsBadRequest(string city)
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = city,
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Broadcaster State Validation

    [Fact]
    //Verificar que el estado tenga un minimo de 4 caracteres
    public async Task RegisterBroadcasterAsync_WhenStateBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "NYC",
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que el estado tenga un maximo de 100 caracteres
    public async Task RegisterBroadcasterAsync_WhenStateExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = new string('a', 101),
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Buenos Aires123")]
    [InlineData("Buenos@Aires")]
    [InlineData("Buenos-Aires")]
    //Verificar que el estado no contenga caracteres no alfabéticos
    public async Task RegisterBroadcasterAsync_WhenStateContainsNonLetters_ReturnsBadRequest(string state)
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = state,
                City = "Ciudad",
                Street = "Calle 123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Broadcaster Street Validation

    [Fact]
    //Verificar que la calle tenga un minimo de 4 caracteres
    public async Task RegisterBroadcasterAsync_WhenStreetBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "123"
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que la calle tenga un maximo de 100 caracteres
    public async Task RegisterBroadcasterAsync_WhenStreetExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterBroadcasterInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = new string('a', 101)
            };

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    // ---------------------------------------------------------------
    // RegisterClientAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterClientAsync_WhenCountryNotFound_ReturnsNotFound()
    {
        var input = DomainBuilders.ClientInput();
        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns((Country?)null);

        var result = await _sut.RegisterClientAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterClientAsync_WhenAgencyDoesNotExist_CreatesNewAgency()
    {
        // Arrange
        var input = DomainBuilders.ClientInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName).Returns((Agency?)null); // no existe
        _hasher.Hash(input.Password).Returns("hashed-password");

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert — una nueva Agency fue creada (agency ??= new Agency(...))
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Clients.Received(1).CreateClient(
            Arg.Is<Client>(c => c.Agency.Name == input.AgencyName));
    }

    [Fact]
    public async Task RegisterClientAsync_WhenAgencyExists_ReusesExistingAgency()
    {
        var existingAgency = new Agency("Existing Agency") { AgencyId = 42 };
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = existingAgency.Name // menos de 3 caracteres
            };
        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName).Returns(existingAgency);
        _hasher.Hash(input.Password).Returns("hashed-password");

        var result = await _sut.RegisterClientAsync(input);

        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Clients.Received(1).CreateClient(
            Arg.Is<Client>(c => c.Agency.AgencyId == 42));
    }

    #region Client Agency Name Validation
    //Verificar que el nombre de agencia tenga un minimo de 3 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenAgencyNameBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "AB" // menos de 3 caracteres
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Verificar que el nombre de agencia tenga un maximo de 100 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenAgencyNameExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = new string('a', 101) // más de 100 caracteres
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Agency123")]
    [InlineData("Agency@")]
    //Verificar que el nombre de agencia no contenga caracteres no alfabéticos
    public async Task RegisterClientAsync_WhenAgencyNameContainsNonLetters_ReturnsBadRequest(string agencyName)
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = agencyName
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client Email Validation
    //Verificar que el email tenga un arroba
    [Fact]
    public async Task RegisterClientAsync_WhenEmailMissingAtSymbol_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "invalidemailexample.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Verificar que el email no esté repetido
    [Fact]
    public async Task RegisterClientAsync_WhenEmailDuplicate_ReturnsConflict()
    {
        // Arrange
        var input = DomainBuilders.ClientInput();
        var existingClient = DomainBuilders.Client();
        existingClient.Email = input.Email;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(Arg.Any<string>()).Returns((Agency?)null);
        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(existingClient);

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    //Verificar que el email tenga un minimo de 10 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenEmailBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "a@b.c",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Verificar que el email tenga un maximo de 100 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenEmailExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = new string('a', 90) + "@example.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client Password Validation
    //Verificar que la contraseña tenga un minimo de 10 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenPasswordBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "short1",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Verificar que la contraseña tenga un maximo de 60 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenPasswordExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = new string('a', 61),
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client First Name Validation
    //Verificar que el nombre tenga un minimo de 3 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenFirstNameBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "Jo",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Verificar que el nombre tenga un maximo de 50 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenFirstNameExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = new string('a', 51),
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Jane123")]
    [InlineData("Jane@")]
    [InlineData("Jane ")]
    //Verificar que el nombre no contenga caracteres no alfabéticos
    public async Task RegisterClientAsync_WhenFirstNameContainsNonLetters_ReturnsBadRequest(string firstName)
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = firstName,
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client Last Name Validation
    //Verificar que el apellido tenga un minimo de 3 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenLastNameBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "Do",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Verificar que el apellido tenga un maximo de 50 caracteres
    [Fact]
    public async Task RegisterClientAsync_WhenLastNameExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = new string('a', 51),
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Smith123")]
    [InlineData("Smith@")]
    [InlineData("Smith ")]
    //Verificar que el apellido no contenga caracteres no alfabéticos
    public async Task RegisterClientAsync_WhenLastNameContainsNonLetters_ReturnsBadRequest(string lastName)
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = lastName,
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client RUT Validation

    [Theory]
    [InlineData("12345678901")] // 11 caracteres
    [InlineData("1234567890123")] // 13 caracteres
    [InlineData("")] // vacío
    //Verificar que el RUT tenga exactamente 12 caracteres
    public async Task RegisterClientAsync_WhenRutInvalidLength_ReturnsBadRequest(string rut)
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = rut,
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que el rut no esté repetido
    public async Task RegisterClientAsync_WhenRutDuplicate_ReturnsConflict()
    {
        // Arrange
        var input = DomainBuilders.ClientInput();
        var existingClient = DomainBuilders.Client();
        existingClient.RUT = input.RUT;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(Arg.Any<string>()).Returns((Agency?)null);
        _unitOfWork.Users.GetUserByRutAsync(input.RUT).Returns(existingClient);

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region Client City Validation

    [Fact]
    //Verificar que la ciudad tenga un minimo de 4 caracteres
    public async Task RegisterClientAsync_WhenCityBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "NYC",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que la ciudad tenga un maximo de 100 caracteres
    public async Task RegisterClientAsync_WhenCityExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = new string('a', 101),
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Buenos Aires123")]
    [InlineData("Buenos@Aires")]
    [InlineData("Buenos-Aires")]
    //Verificar que la ciudad no contenga caracteres no alfabéticos
    public async Task RegisterClientAsync_WhenCityContainsNonLetters_ReturnsBadRequest(string city)
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = city,
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client State Validation

    [Fact]
    //Verificar que el estado tenga un minimo de 4 caracteres
    public async Task RegisterClientAsync_WhenStateBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "NYC",
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que el estado tenga un maximo de 100 caracteres
    public async Task RegisterClientAsync_WhenStateExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = new string('a', 101),
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("Buenos Aires123")]
    [InlineData("Buenos@Aires")]
    [InlineData("Buenos-Aires")]
    //Verificar que el estado no contenga caracteres no alfabéticos
    public async Task RegisterClientAsync_WhenStateContainsNonLetters_ReturnsBadRequest(string state)
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = state,
                City = "Ciudad",
                Street = "Calle 123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Client Street Validation

    [Fact]
    //Verificar que la calle tenga un minimo de 4 caracteres
    public async Task RegisterClientAsync_WhenStreetBelowMinimumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = "123",
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    //Verificar que la calle tenga un maximo de 100 caracteres
    public async Task RegisterClientAsync_WhenStreetExceedsMaximumLength_ReturnsBadRequest()
    {
        // Arrange
        var input = new RegisterClientInput() {
                Email = "prueba@ejemplo.com",
                Password = "Password123!",
                FirstName = "PrimerNombre",
                LastName = "PrimerApellido",
                RUT = "123456789012",
                CountryCode = "UY",
                State = "Estado",
                City = "Ciudad",
                Street = new string('a', 101),
                AgencyName = "TestAgency"
            };

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    // ---------------------------------------------------------------
    // LoginAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var input = new UserLoginInput { Email = "test@alpu.uy", Password = "correct-pass" };
        var user = DomainBuilders.Client();
        user.Email = input.Email;
        user.Password = "hashed-pass";

        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(user);
        _hasher.Verify(input.Password, user.Password).Returns(true);

        var result = await _sut.LoginAsync(input);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNotFound()
    {
        var input = new UserLoginInput { Email = "test@alpu.uy", Password = "wrong-pass" };
        var user = DomainBuilders.Client();
        user.Password = "hashed-pass";

        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(user);
        _hasher.Verify(input.Password, user.Password).Returns(false); // wrong password

        var result = await _sut.LoginAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LoginAsync_WhenUserRegisteredWithGoogle_ReturnsNotFound()
    {
        // Password is null → registered via Google, should not login with email
        var input = new UserLoginInput { Email = "google@alpu.uy", Password = "any" };
        var user = DomainBuilders.Client();
        user.Password = null;

        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(user);

        var result = await _sut.LoginAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorized()
    {
        var input = new UserLoginInput { Email = "noexiste@alpu.uy", Password = "pass" };
        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns((User?)null);

        // El servicio hace ?? throw new UnauthorizedAccessException(...)
        var act = () => _sut.LoginAsync(input);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*incorrectos*");
    }

    // ---------------------------------------------------------------
    // GoogleAuthAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GoogleAuthAsync_WithInvalidToken_ReturnsNotFound()
    {
        var input = new GoogleAuthInput { Code = "invalid-code" };
        _googleAuthService.ExchangeCodeAsync(input.Code).Returns((GoogleUserInfo?)null);

        var result = await _sut.GoogleAuthAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GoogleAuthAsync_WhenUserExists_ReturnsTokenWithoutRequiringRegistration()
    {
        var input = new GoogleAuthInput { Code = "valid-code" };
        var googleInfo = new GoogleUserInfo
        {
            Subject = "google-sub-123",
            Email = "user@alpu.uy",
            GivenName = "Tadeo",
            FamilyName = "Mieres"
        };

        var existingUser = DomainBuilders.Client();
        existingUser.GoogleId = "google-sub-123";
        existingUser.Email = googleInfo.Email;

        _googleAuthService.ExchangeCodeAsync(input.Code).Returns(googleInfo);
        _unitOfWork.Users.GetUserByGoogleIdAsync(googleInfo.Subject).Returns(existingUser);

        var result = await _sut.GoogleAuthAsync(input);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RequiresRegistration.Should().BeFalse();
        result.Value.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GoogleAuthAsync_WhenUserNotFound_ReturnsRequiresRegistrationTrue()
    {
        var input = new GoogleAuthInput { Code = "valid-code" };
        var googleInfo = new GoogleUserInfo
        {
            Subject = "new-google-sub",
            Email = "new@alpu.uy",
            GivenName = "Martin",
            FamilyName = "Almeida"
        };

        _googleAuthService.ExchangeCodeAsync(input.Code).Returns(googleInfo);
        _unitOfWork.Users.GetUserByGoogleIdAsync(googleInfo.Subject).Returns((User?)null);

        var result = await _sut.GoogleAuthAsync(input);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RequiresRegistration.Should().BeTrue();
        result.Value.Token.Should().BeNull();
    }

    [Fact]
    public async Task GoogleAuthAsync_WhenExistingUserHasNoGoogleId_LinksGoogleId()
    {
        // Usuario que se registró con email/password primero, luego entra con Google
        var input = new GoogleAuthInput { Code = "valid-code" };
        var googleInfo = new GoogleUserInfo { Subject = "google-sub-456", Email = "old@alpu.uy" };
        var userWithoutGoogleId = DomainBuilders.Client();
        userWithoutGoogleId.GoogleId = null;
        userWithoutGoogleId.Email = googleInfo.Email;

        _googleAuthService.ExchangeCodeAsync(input.Code).Returns(googleInfo);
        _unitOfWork.Users.GetUserByGoogleIdAsync(googleInfo.Subject).Returns(userWithoutGoogleId);

        await _sut.GoogleAuthAsync(input);

        // GoogleId must be linked
        userWithoutGoogleId.GoogleId.Should().Be(googleInfo.Subject);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}

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
using Tests.Helpers;
using Domain.Common.Errors;

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
    public async Task RegisterBroadcasterAsync_WhenCountryNotFound_ReturnsFail()
    {
        // Arrange
        var input = InputBuilders.ValidBroadcasterInput();

        _unitOfWork.Broadcasters.GetCategoryByIdAsync(1).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns((Country?)null);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasError<CountryErrors.CountryNotFoundError>();
    }

    [Fact]
    public async Task RegisterBroadcasterAsync_WhenCategoryNotFound_ThrowsException()
    {
        // Arrange
        var input = InputBuilders.ValidBroadcasterInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country());
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(1).Returns((BroadcasterCategory?)null);

        // Act
        var act = async () => await _sut.RegisterBroadcasterAsync(input);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RegisterBroadcasterAsync_WithValidInput_SavesBroadcasterAndReturnsToken()
    {
        // Arrange
        var input = InputBuilders.ValidBroadcasterInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Departments.GetByIdAsync(input.DepartmentId).Returns(new Department { DepartmentId = 1 });
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(1).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _hasher.Hash(input.Password).Returns("hashed-password");

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBeNullOrEmpty();

        // Verify persistence
        _unitOfWork.Broadcasters.Received(1).CreateBroadcaster(Arg.Any<Broadcaster>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    //Verificar que el email no esté repetido
    [Fact]
    public async Task RegisterBroadcasterAsync_WhenEmailDuplicate_ReturnsFail()
    {
        // Arrange
        var input = InputBuilders.ValidBroadcasterInput();
        var existingBroadcaster = DomainBuilders.ValidBroadcaster();
        existingBroadcaster.Email = input.Email;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(Arg.Any<int>()).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(existingBroadcaster);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.DuplicatedEmailError>();
    }

    // ---------------------------------------------------------------
    // RegisterClientAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterClientAsync_WhenCountryNotFound_ReturnsFail()
    {
        var input = InputBuilders.ValidClientInput();
        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns((Country?)null);

        var result = await _sut.RegisterClientAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.HasError<CountryErrors.CountryNotFoundError>();
    }

    [Fact]
    public async Task RegisterClientAsync_WhenAgencyDoesNotExist_CreatesNewAgency()
    {
        // Arrange
        var input = InputBuilders.ValidClientInput();

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Departments.GetByIdAsync(input.DepartmentId).Returns(new Department { DepartmentId = 1 });
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
        var input = new RegisterClientInput()
        {
            Email = "prueba@ejemplo.com",
            Password = "Password123!",
            FirstName = "PrimerNombre",
            LastName = "PrimerApellido",
            RUT = "123456789012",
            CountryCode = "UY",
            DepartmentId = 1,
            City = "Ciudad",
            Street = "Calle 123",
            AgencyName = existingAgency.Name // menos de 3 caracteres
        };

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Departments.GetByIdAsync(input.DepartmentId).Returns(new Department { DepartmentId = 1 });
        _unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName).Returns(existingAgency);
        _hasher.Hash(input.Password).Returns("hashed-password");

        var result = await _sut.RegisterClientAsync(input);

        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Clients.Received(1).CreateClient(
            Arg.Is<Client>(c => c.Agency.AgencyId == 42));
    }

    [Fact]
    public async Task RegisterClientAsync_WhenEmailDuplicate_ReturnsFail()
    {
        // Arrange
        var input = InputBuilders.ValidClientInput();
        var existingClient = DomainBuilders.ValidClient();
        existingClient.Email = input.Email;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(Arg.Any<string>()).Returns((Agency?)null);
        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(existingClient);

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.DuplicatedEmailError>();
    }

    [Fact]
    //Verificar que el rut no esté repetido
    public async Task RegisterClientAsync_WhenRutDuplicate_ReturnsFail()
    {
        // Arrange
        var input = InputBuilders.ValidClientInput();
        var existingClient = DomainBuilders.ValidClient();
        existingClient.RUT = input.RUT;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(Arg.Any<string>()).Returns((Agency?)null);
        _unitOfWork.Users.GetUserByRutAsync(input.RUT).Returns(existingClient);

        // Act
        var result = await _sut.RegisterClientAsync(input);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.DuplicatedEmailError>();
    }

    // ---------------------------------------------------------------
    // LoginAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var input = new UserLoginInput { Email = "test@alpu.uy", Password = "correct-pass" };
        var user = DomainBuilders.ValidClient();
        user.Email = input.Email;
        user.Password = "hashed-pass";

        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(user);
        _hasher.Verify(input.Password, user.Password).Returns(true);

        var result = await _sut.LoginAsync(input);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFail()
    {
        var input = new UserLoginInput { Email = "test@alpu.uy", Password = "wrong-pass" };
        var user = DomainBuilders.ValidClient();
        user.Password = "hashed-pass";

        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(user);
        _hasher.Verify(input.Password, user.Password).Returns(false); // wrong password

        var result = await _sut.LoginAsync(input);

        result.IsFailed.Should().BeTrue();
        result.HasError<AuthError>();
    }

    [Fact]
    public async Task LoginAsync_WhenUserRegisteredWithGoogle_ReturnsFail()
    {
        // Password is null → registered via Google, should not login with email
        var input = new UserLoginInput { Email = "google@alpu.uy", Password = "any" };
        var user = DomainBuilders.ValidClient();
        user.Password = null;

        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns(user);

        var result = await _sut.LoginAsync(input);

        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.GoogleUserTryNormalLoginError>();
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ReturnsFail()
    {
        var input = new UserLoginInput { Email = "noexiste@alpu.uy", Password = "pass" };
        _unitOfWork.Users.GetUserByEmailAsync(input.Email).Returns((User?)null);

        // El servicio hace ?? throw new UnauthorizedAccessException(...)
        var result = await _sut.LoginAsync(input);

        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.UserNotFoundError>();
    }

    // ---------------------------------------------------------------
    // GoogleAuthAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GoogleAuthAsync_WithInvalidToken_ReturnsFail()
    {
        var input = new GoogleAuthInput { Code = "invalid-code" };
        _googleAuthService.ExchangeCodeAsync(input.Code).Returns((GoogleUserInfo?)null);

        var result = await _sut.GoogleAuthAsync(input);

        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.GoogleTokenIsInvalidError>();
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

        var existingUser = DomainBuilders.ValidClient();
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
        var userWithoutGoogleId = DomainBuilders.ValidClient();
        userWithoutGoogleId.GoogleId = null;
        userWithoutGoogleId.Email = googleInfo.Email;

        _googleAuthService.ExchangeCodeAsync(input.Code).Returns(googleInfo);
        _unitOfWork.Users.GetUserByGoogleIdAsync(googleInfo.Subject).Returns(userWithoutGoogleId);

        await _sut.GoogleAuthAsync(input);

        // GoogleId must be linked
        userWithoutGoogleId.GoogleId.Should().Be(googleInfo.Subject);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GoogleAuthAsync_WhenGoogleCodeIsNull_ReturnsFail()
    {
        // Usuario que se registró con email/password primero, luego entra con Google
        var input = new GoogleAuthInput { Code = "" };
        var googleInfo = new GoogleUserInfo { Subject = "google-sub-456", Email = "old@alpu.uy" };
        var userWithoutGoogleId = DomainBuilders.ValidClient();
        userWithoutGoogleId.GoogleId = null;
        userWithoutGoogleId.Email = googleInfo.Email;

        _googleAuthService.ExchangeCodeAsync(input.Code).Returns((GoogleUserInfo?)null);

        var result = await _sut.GoogleAuthAsync(input);

        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.GoogleIdIsRequiredError>();
    }

    [Fact]
    //Verificar que el rut no esté repetido
    public async Task RegisterBroadcasterAsync_WhenRutDuplicate_ReturnsFail()
    {
        // Arrange
        var input = InputBuilders.ValidBroadcasterInput();
        var existingBroadcaster = DomainBuilders.ValidBroadcaster();
        existingBroadcaster.RUT = input.RUT;

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Broadcasters.GetCategoryByIdAsync(Arg.Any<int>()).Returns(new BroadcasterCategory { BroadcasterCategoryId = 1 });
        _unitOfWork.Users.GetUserByRutAsync(input.RUT).Returns(existingBroadcaster);

        // Act
        var result = await _sut.RegisterBroadcasterAsync(input);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.HasError<UserErrors.DuplicatedRutError>();
    }
}

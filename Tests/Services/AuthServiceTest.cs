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
        var input = DomainBuilders.ClientInput();
        var existingAgency = new Agency("Existing Agency") { AgencyId = 42 };

        _unitOfWork.Countries.GetByCodeAsync(input.CountryCode).Returns(new Country { CountryCode = "UY" });
        _unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName).Returns(existingAgency);
        _hasher.Hash(input.Password).Returns("hashed-password");

        var result = await _sut.RegisterClientAsync(input);

        result.IsSuccess.Should().BeTrue();
        _unitOfWork.Clients.Received(1).CreateClient(
            Arg.Is<Client>(c => c.Agency.AgencyId == 42));
    }

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

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    // private static RegisterBroadcasterInput BuildBroadcasterInput() => new()
    // {
    //     FirstName = "Tadeo",
    //     LastName = "Mieres",
    //     Email = "tadeo@alpu.uy",
    //     Password = "Password123!",
    //     CountryCode = "UY",
    // };

    // private static RegisterClientInput BuildClientInput() => new()
    // {
    //     FirstName = "Martin",
    //     LastName = "Almeida",
    //     Email = "martin@agency.uy",
    //     Password = "Password123!",
    //     CountryCode = "UY",
    //     AgencyName = "Test Agency",
    // };
}

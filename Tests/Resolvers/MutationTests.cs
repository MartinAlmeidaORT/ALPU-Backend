// Resolvers/MutationTests.cs
using Application.Common;
using Application.Interfaces.Public.Services;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using FluentAssertions;
using GraphQL.Schema;
using HotChocolate;
using NSubstitute;
using Tests.Helpers;

namespace Tests.Resolvers;

public class MutationTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly Mutation _sut = new();

    // ---------------------------------------------------------------
    // RegisterBroadcaster
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterBroadcaster_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = DomainBuilders.BroadcasterInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.Broadcaster());

        _authService.RegisterBroadcasterAsync(input)
            .Returns(ResultAPI<AuthPayload>.Success(payload));

        var result = await _sut.RegisterBroadcaster(_authService, input);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task RegisterBroadcaster_WhenServiceFails_ThrowsException()
    {
        var input = DomainBuilders.BroadcasterInput();

        _authService.RegisterBroadcasterAsync(input)
            .Returns(ResultAPI<AuthPayload>.NotFound("Country not found."));

        var act = () => _sut.RegisterBroadcaster(_authService, input);

        // UnwrapOrThrow() debe lanzar cuando IsSuccess = false
        await act.Should().ThrowAsync<GraphQLException>();
    }

    // ---------------------------------------------------------------
    // RegisterClient
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterClient_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = DomainBuilders.ClientInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.Client());

        _authService.RegisterClientAsync(input)
            .Returns(ResultAPI<AuthPayload>.Success(payload));

        var result = await _sut.RegisterClient(_authService, input);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task RegisterClient_WhenServiceFails_ThrowsException()
    {
        var input = DomainBuilders.ClientInput();

        _authService.RegisterClientAsync(input)
            .Returns(ResultAPI<AuthPayload>.BadRequest("Validation error."));

        var act = () => _sut.RegisterClient(_authService, input);

        await act.Should().ThrowAsync<GraphQLException>();
    }

    // ---------------------------------------------------------------
    // LoginAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task LoginAsync_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = new UserLoginInput { Email = "test@alpu.uy", Password = "pass" };
        var payload = new AuthPayload("jwt-token", DomainBuilders.Client());

        _authService.LoginAsync(input)
            .Returns(ResultAPI<AuthPayload>.Success(payload));

        var result = await _sut.LoginAsync(_authService, input);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsInvalid_ThrowsException()
    {
        var input = new UserLoginInput { Email = "bad@alpu.uy", Password = "wrong" };

        _authService.LoginAsync(input)
            .Returns(ResultAPI<AuthPayload>.NotFound("Email o contraseña incorrectos."));

        var act = () => _sut.LoginAsync(_authService, input);

        await act.Should().ThrowAsync<GraphQLException>();
    }

    // ---------------------------------------------------------------
    // GoogleAuthAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GoogleAuthAsync_WhenServiceSucceeds_ReturnsGooglePayload()
    {
        var input = new GoogleAuthInput { Code = "valid-code" };
        var payload = new GoogleAuthPayload { RequiresRegistration = false, Token = "jwt" };

        _authService.GoogleAuthAsync(input)
            .Returns(ResultAPI<GoogleAuthPayload>.Success(payload));

        var result = await _sut.GoogleAuthAsync(input, _authService);

        result.RequiresRegistration.Should().BeFalse();
        result.Token.Should().Be("jwt");
    }

    [Fact]
    public async Task GoogleAuthAsync_WithInvalidCode_ThrowsException()
    {
        var input = new GoogleAuthInput { Code = "bad-code" };

        _authService.GoogleAuthAsync(input)
            .Returns(ResultAPI<GoogleAuthPayload>.NotFound("Token de Google inválido."));

        var act = () => _sut.GoogleAuthAsync(input, _authService);

        await act.Should().ThrowAsync<GraphQLException>();
    }

    // ---------------------------------------------------------------
    // CompleteGoogleSignUp — Broadcaster & Client
    // ---------------------------------------------------------------

    [Fact]
    public async Task CompleteGoogleSignUpBroadcaster_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = DomainBuilders.GoogleBroadcasterInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.BroadcasterFromGoogle());

        _authService.CompleteGoogleSignUpBroadcasterAsync(input)
            .Returns(ResultAPI<AuthPayload>.Success(payload));

        var result = await _sut.CompleteGoogleSignUpBroadcasterAsync(input, _authService);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task CompleteGoogleSignUpClient_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = DomainBuilders.GoogleClientInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.ClientFromGoogle());

        _authService.CompleteGoogleSignUpClientAsync(input)
            .Returns(ResultAPI<AuthPayload>.Success(payload));

        var result = await _sut.CompleteGoogleSignUpClientAsync(input, _authService);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task CompleteGoogleSignUpBroadcaster_WhenCountryNotFound_ThrowsException()
    {
        var input = DomainBuilders.GoogleBroadcasterInput();

        _authService.CompleteGoogleSignUpBroadcasterAsync(input)
            .Returns(ResultAPI<AuthPayload>.NotFound("Country not found."));

        var act = () => _sut.CompleteGoogleSignUpBroadcasterAsync(input, _authService);

        await act.Should().ThrowAsync<GraphQLException>();
    }
}

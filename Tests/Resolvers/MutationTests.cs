// Resolvers/MutationTests.cs
using Application.Interfaces.Public.Services;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using Domain.Models;
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
        var input = InputBuilders.ValidBroadcasterInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.ValidBroadcaster());

        _authService.RegisterBroadcasterAsync(input).Returns(payload);

        var result = await _sut.RegisterBroadcaster(_authService, input);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task RegisterBroadcaster_WhenServiceFails_ThrowsException()
    {
        var input = InputBuilders.ValidBroadcasterInput();

        _authService.RegisterBroadcasterAsync(input).Returns(CountryErrors.CountryNotFound(input.CountryCode));

        var act = () => _sut.RegisterBroadcaster(_authService, input);

        await act.Should().ThrowAsync<GraphQLException>();
    }

    // ---------------------------------------------------------------
    // RegisterClient
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterClient_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = InputBuilders.ValidClientInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.ValidClient());

        _authService.RegisterClientAsync(input).Returns(payload);

        var result = await _sut.RegisterClient(_authService, input);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task RegisterClient_WhenServiceFails_ThrowsException()
    {
        var input = InputBuilders.ValidClientInput();

        _authService.RegisterClientAsync(input).Returns(UserErrors.DuplicatedEmail(input.Email));

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
        var payload = new AuthPayload("jwt-token", DomainBuilders.ValidClient());

        _authService.LoginAsync(input).Returns(payload);

        var result = await _sut.LoginAsync(_authService, input);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsInvalid_ThrowsException()
    {
        var input = new UserLoginInput { Email = "bad@alpu.uy", Password = "wrong" };

        _authService.LoginAsync(input).Returns(UserErrors.LoginFailed());

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

        _authService.GoogleAuthAsync(input).Returns(payload);

        var result = await _sut.GoogleAuthAsync(input, _authService);

        result.RequiresRegistration.Should().BeFalse();
        result.Token.Should().Be("jwt");
    }

    [Fact]
    public async Task GoogleAuthAsync_WithInvalidCode_ThrowsException()
    {
        var input = new GoogleAuthInput { Code = "bad-code" };

        _authService.GoogleAuthAsync(input).Returns(UserErrors.GoogleTokenIsInvalid());

        var act = () => _sut.GoogleAuthAsync(input, _authService);

        await act.Should().ThrowAsync<GraphQLException>();
    }

    // ---------------------------------------------------------------
    // CompleteGoogleSignUp — Broadcaster & Client
    // ---------------------------------------------------------------

    [Fact]
    public async Task CompleteGoogleSignUpBroadcaster_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = InputBuilders.ValidGoogleBroadcasterInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.ValidBroadcasterFromGoogle());

        _authService.CompleteGoogleSignUpBroadcasterAsync(input).Returns(payload);

        var result = await _sut.CompleteGoogleSignUpBroadcasterAsync(input, _authService);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task CompleteGoogleSignUpClient_WhenServiceSucceeds_ReturnsPayload()
    {
        var input = InputBuilders.ValidGoogleClientInput();
        var payload = new AuthPayload("jwt-token", DomainBuilders.ValidClientFromGoogle());

        _authService.CompleteGoogleSignUpClientAsync(input).Returns(payload);

        var result = await _sut.CompleteGoogleSignUpClientAsync(input, _authService);

        result.Should().BeEquivalentTo(payload);
    }

    [Fact]
    public async Task CompleteGoogleSignUpBroadcaster_WhenCountryNotFound_ThrowsException()
    {
        var input = InputBuilders.ValidGoogleBroadcasterInput();

        _authService.CompleteGoogleSignUpBroadcasterAsync(input).Returns(CountryErrors.CountryNotFound(input.CountryCode));

        var act = () => _sut.CompleteGoogleSignUpBroadcasterAsync(input, _authService);

        await act.Should().ThrowAsync<GraphQLException>();
    }
}

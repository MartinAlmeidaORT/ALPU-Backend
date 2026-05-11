using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Public.Services;
using Domain.Interfaces.Public.Repositories;
using Domain.Common;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Domain.Interfaces.Private;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using FluentResults;

namespace Application.Services;

public class AuthService(IHasher hasher, IUnitOfWork unitOfWork, IConfiguration configuration, IGoogleAuthService googleAuthService) : IAuthService
{
    private readonly IConfiguration _config = configuration;

    public async Task<Result<AuthPayload>> RegisterBroadcasterAsync(RegisterBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);

        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1)
        ?? throw new ArgumentNullException($"La categoria de locutor {1} no existe en la base de datos.");

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return UserErrors.DuplicatedRUT(input.RUT);
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return UserErrors.DuplicatedEmail(input.Email);
        }

        if (country is null)
        {
            return CountryErrors.CountryNotFound(input.CountryCode);
        }

        Result<Broadcaster> result = Broadcaster.SignUp(input, country, category, hasher.Hash(input.Password));

        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Broadcasters.CreateBroadcaster(result.Value);
        await unitOfWork.SaveChangesAsync();

        return Result.Ok(new AuthPayload(GenerateJWT(result.Value), result.Value));
    }

    public async Task<Result<AuthPayload>> RegisterClientAsync(RegisterClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= new Agency(input.AgencyName);

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return UserErrors.DuplicatedRUT(input.RUT);
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return UserErrors.DuplicatedEmail(input.Email);
        }

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);

        Result<Client> result = Client.SignUp(input, country, agency, hasher.Hash(input.Password));

        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Clients.CreateClient(result.Value);
        await unitOfWork.SaveChangesAsync();

        return Result.Ok(new AuthPayload(GenerateJWT(result.Value), result.Value));
    }

    public async Task<Result<AuthPayload>> LoginAsync(UserLoginInput input)
    {
        User? user = await unitOfWork.Users.GetUserByEmailAsync(input.Email);

        if (user is null) return UserErrors.LoginFailed();

        if (user.Password is null) return UserErrors.GoogleUserTryNormalLogin();

        if (!hasher.Verify(input.Password, user.Password)) return UserErrors.LoginFailed();

        return Result.Ok(new AuthPayload(GenerateJWT(user), user));
    }

    public async Task<Result<GoogleAuthPayload>> GoogleAuthAsync(GoogleAuthInput input)
    {
        // Validar token con Google
        GoogleUserInfo? payload = await googleAuthService.ExchangeCodeAsync(input.Code);

        if (payload is null) return UserErrors.GoogleTokenIsInvalid();

        User? user = await unitOfWork.Users.GetUserByGoogleIdAsync(payload.Subject);

        // Vincula GoogleId si entró antes con email / password
        if (user is not null && user.GoogleId is null)
        {
            user.GoogleId = payload.Subject;
            await unitOfWork.SaveChangesAsync();
        }

        return Result.Ok(new GoogleAuthPayload
        {
            Token = user is not null ? GenerateJWT(user) : null,
            RequiresRegistration = user is null,
            Subject = payload.Subject,
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
        });
    }

    public async Task<Result<AuthPayload>> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);

        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1)
        ?? throw new ArgumentNullException($"La categoria de locutor {1} no existe en la base de datos.");

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);

        Result<Broadcaster> result = Broadcaster.SignUpFromGoogle(input, country, category);
        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Broadcasters.CreateBroadcaster(result.Value);
        await unitOfWork.SaveChangesAsync();

        return Result.Ok(new AuthPayload(GenerateJWT(result.Value), result.Value));
    }

    public async Task<Result<AuthPayload>> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= new Agency(input.AgencyName);

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);

        Result<Client> result = Client.SignUpFromGoogle(input, country, agency);
        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Clients.CreateClient(result.Value);
        await unitOfWork.SaveChangesAsync();

        return Result.Ok(new AuthPayload(GenerateJWT(result.Value), result.Value));
    }

    private string GenerateJWT(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name,               $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role,               user.GetType().Name), // "Client", "Broadcaster"
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

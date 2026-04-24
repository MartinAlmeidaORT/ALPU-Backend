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

namespace Application.Services;

public class AuthService(IHasher hasher, IUnitOfWork unitOfWork, IConfiguration configuration, IGoogleAuthService googleAuthService) : IAuthService
{
    private readonly IConfiguration _config = configuration;

    public async Task<AuthPayload> RegisterBroadcasterAsync(RegisterBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");
        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1) ?? throw new KeyNotFoundException($"Category with id {1} not found.");

        Broadcaster broadcaster = new(input, country, category)
        {
            Password = hasher.Hash(input.Password)
        };

        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(broadcaster), broadcaster);
    }

    public async Task<AuthPayload> RegisterClientAsync(RegisterClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= unitOfWork.Clients.CreateAgency(new Agency(input.AgencyName));

        Client client = new(input, country, agency)
        {
            Password = hasher.Hash(input.Password)
        };

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(client), client);
    }

    public async Task<AuthPayload> LoginAsync(UserLoginInput input)
    {
        User user = await unitOfWork.Users.GetUserByEmailAsync(input.Email) ?? throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

        if (user.Password is null)
            throw new ArgumentException("El usuario deberia ingresar con su cuenta de Google.");

        if (!hasher.Verify(input.Password, user.Password))
            throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

        return new AuthPayload(GenerateJWT(user), user);
    }

    public async Task<GoogleAuthPayload> GoogleAuthAsync(GoogleAuthInput input)
    {
        // Validar token con Google
        GoogleUserInfo payload = await googleAuthService.ExchangeCodeAsync(input.Code)
        ?? throw new UnauthorizedAccessException("Token de Google inválido.");

        User? user = await unitOfWork.Users.GetUserByGoogleIdAsync(payload.Subject);

        // Vincula GoogleId si entró antes con email / password
        if (user is not null && user.GoogleId is null)
        {
            user.GoogleId = payload.Subject;
            await unitOfWork.SaveChangesAsync();
        }

        return new GoogleAuthPayload
        {
            Token = user is not null ? GenerateJWT(user) : null,
            RequiresRegistration = user is null,
            Subject = payload.Subject,
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
        };
    }

    public async Task<AuthPayload> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");
        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1) ?? throw new KeyNotFoundException($"Category with id {1} not found.");

        Broadcaster broadcaster = new(input, country, category);

        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(broadcaster), broadcaster);
    }

    public async Task<AuthPayload> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= unitOfWork.Clients.CreateAgency(new Agency(input.AgencyName));

        Client client = new(input, country, agency);

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(client), client);
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

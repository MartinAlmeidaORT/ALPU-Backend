using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;
using Application.Interfaces.Public.Services;
using Application.Mappers;
using Domain.Interfaces.Public.Repositories;
using Domain.Common;
using Domain.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration) : IAuthService
{
    private readonly IConfiguration _config = configuration;

    public async Task<AuthPayload> RegisterBroadcasterAsync(CreateBroadcasterDTO input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");
        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1) ?? throw new KeyNotFoundException($"Category with id {1} not found.");

        Broadcaster broadcaster = BroadcasterMapper.ToEntity(input, country);

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
        broadcaster.Password = hashedPassword;
        broadcaster.Address.Country = country;
        broadcaster.Category = category;

        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(broadcaster), broadcaster);
    }

    public async Task<AuthPayload> RegisterClientAsync(CreateClientDTO input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");

        Client client = ClientMapper.ToEntity(input);

        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= unitOfWork.Clients.CreateAgency(new Agency(input.AgencyName));

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
        client.Password = hashedPassword;
        client.Agency = agency;
        client.Address.Country = country;

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(client), client);
    }

    public async Task<AuthPayload> LoginAsync(LoginUserInput input)
    {
        var user = await unitOfWork.Users.GetUserByEmailAsync(input.Email) ?? throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

        if (!BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

        return new AuthPayload(GenerateJWT(user), user);
    }

    public async Task<AuthPayload> GoogleAuthAsync(GoogleAuthInput input)
    {
        // Validar token con Google
        var payload = await ValidateGoogleTokenAsync(input.Token) ?? throw new UnauthorizedAccessException("Token de Google inválido.");
        var user = await unitOfWork.Users.GetUserByGoogleIdOrEmailAsync(payload.Subject, payload.Email);

        return input switch
        {
            // Registro - usuario nuevo
            RegisterClientGoogleDTO i => await HandleGoogleRegisterAsync(user, payload, i),
            RegisterBroadcasterGoogleDTO i => await HandleGoogleRegisterAsync(user, payload, i),
            // Login - usuario existente
            GoogleAuthInput => await HandleGoogleLoginAsync(user, payload),
            _ => throw new ArgumentException("Tipo de operación inválido.")
        };
    }

    private async Task<AuthPayload> HandleGoogleLoginAsync(User? user,
        GoogleJsonWebSignature.Payload payload)
    {
        // Si manda GoogleAuthDTO pero no existe, le decimos que se registre
        if (user is null)
            throw new UnauthorizedAccessException("No existe una cuenta con este email. Por favor registrese.");

        // Vincula GoogleId si entró antes con email / password
        if (user.GoogleId is null)
        {
            user.GoogleId = payload.Subject;
            await unitOfWork.SaveChangesAsync();
        }

        return new AuthPayload(GenerateJWT(user), user);
    }

    private async Task<AuthPayload> HandleGoogleRegisterAsync(User? user,
        GoogleJsonWebSignature.Payload payload,
        RegisterUserGoogleDTO input)
    {
        if (user is not null)
            throw new UnauthorizedAccessException("Ya existe una cuenta con este email.");

        // Campos que vienen del token de Google
        User newUser = input switch
        {
            RegisterClientGoogleDTO i => ClientMapper.ToEntity(i),
            RegisterBroadcasterGoogleDTO i => BroadcasterMapper.ToEntity(i),
            _ => throw new ArgumentException("Tipo de registro inválido.")
        };

        Country country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");
        newUser.Address.Country = country;
        newUser.Address.CountryCode = country.CountryCode;

        // Campos comunes — vienen del token, no del input
        newUser.Email = payload.Email.Trim().ToLower();
        newUser.GoogleId = payload.Subject;

        switch (newUser)
        {
            case Client newClient:
                RegisterClientGoogleDTO clientInput = (RegisterClientGoogleDTO)input;
                Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(clientInput.AgencyName);
                agency ??= unitOfWork.Clients.CreateAgency(new(clientInput.AgencyName));
                newClient.Agency = agency;
                unitOfWork.Clients.CreateClient(newClient);
                break;
            case Broadcaster newBroadcaster:
                unitOfWork.Broadcasters.CreateBroadcaster(newBroadcaster);
                break;
            default:
                throw new ArgumentException("Tipo de registro inválido.");
        }

        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(GenerateJWT(newUser), newUser);
    }

    private async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string token)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_config["Google:ClientId"]]
            };
            return await GoogleJsonWebSignature.ValidateAsync(token, settings);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
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

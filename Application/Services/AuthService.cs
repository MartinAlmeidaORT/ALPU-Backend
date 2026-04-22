using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;
using Application.DTOs.Users;
using Application.Interfaces.Public.Services;
using Application.Mappers;
using Domain.Interfaces.Public.Repositories;
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

        broadcaster.Address.Country = country;
        broadcaster.Category = category;

        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload("", BroadcasterMapper.ToDTO(broadcaster), null);
    }

    public async Task<AuthPayload> RegisterClientAsync(CreateClientDTO input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode) ?? throw new KeyNotFoundException($"Country with code {input.CountryCode} not found.");

        Client client = ClientMapper.ToEntity(input);

        Agency agency = unitOfWork.Clients.CreateAgency(new(input.AgencyName));

        client.Agency = agency;
        client.Address.Country = country;

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();

        return new AuthPayload("", ClientMapper.ToDTO(client), null);
    }

    public async Task<AuthPayload> GoogleAuthAsync(GoogleAuthInput input)
    {
        // Validar token con Google
        var payload = await ValidateGoogleTokenAsync(input.Token);
        if (payload is null)
            return AuthPayload.Fail("Token de Google inválido.");

        var user = await unitOfWork.Users.GetUserByGoogleIdOrEmailAsync(payload.Subject, payload.Email);

        return input switch
        {
            // Registro - usuario nuevo
            RegisterClientGoogleDTO i => await HandleGoogleRegisterAsync(user, payload, i),
            RegisterBroadcasterGoogleDTO i => await HandleGoogleRegisterAsync(user, payload, i),
            // Login - usuario existente
            GoogleAuthInput => await HandleGoogleLoginAsync(user, payload),
            _ => AuthPayload.Fail("Tipo de operación inválido.")
        };
    }

    public Task<AuthPayload> LoginAsync(LoginUserInput input)
    {
        throw new NotImplementedException();
    }

    private async Task<AuthPayload> HandleGoogleLoginAsync(User? user,
        GoogleJsonWebSignature.Payload payload)
    {
        // Si manda GoogleAuthDTO pero no existe, le decimos que se registre
        if (user is null)
            return AuthPayload.Fail("No existe una cuenta con este email. Por favor registrese.");

        // Vincula GoogleId si entró antes con email / password
        if (user.GoogleId is null)
        {
            user.GoogleId = payload.Subject;
            await unitOfWork.SaveChangesAsync();
        }

        // if (user.UserState == Domain.Enums.UserState.Pending)
        //     return AuthPayload.Fail("Su cuenta está pendiente de aprobación.");

        ResultUserDTO userPayload = user switch
        {
            Broadcaster broadcaster => BroadcasterMapper.ToDTO(broadcaster),
            Client client => ClientMapper.ToDTO(client),
            _ => throw new ArgumentException("Tipo de usuario inválido.")
        };

        return new AuthPayload(GenerateJWT(user), userPayload, null);
    }

    private async Task<AuthPayload> HandleGoogleRegisterAsync(User? user,
        GoogleJsonWebSignature.Payload payload,
        RegisterUserGoogleDTO input)
    {
        if (user is not null)
            return AuthPayload.Fail("Ya existe una cuenta con este email.");

        // Campos que vienen del token de Google
        User newUser = input switch
        {
            RegisterClientGoogleDTO i => new Client
            {
                Agency = new Agency(i.AgencyName)
            },
            RegisterBroadcasterGoogleDTO i => BroadcasterMapper.ToEntity(i),
            _ => throw new ArgumentException("Tipo de registro inválido.")
        };

        // Campos comunes — vienen del token, no del input
        newUser.Email = payload.Email.Trim().ToLower();
        newUser.FirstName = payload.GivenName ?? string.Empty;
        newUser.LastName = payload.FamilyName ?? string.Empty;
        newUser.GoogleId = payload.Subject;
        newUser.UserState = Domain.Enums.UserState.Pending;

        // Campos del dominio — vienen del input
        // newUser.RUT = input.RUT;
        // newUser.Address.CountryCode = input.CountryCode;
        // newUser.Address.State = input.State;
        // newUser.Address.City = input.City;
        // newUser.Address.Street = input.Street;

        // TODO
        // newUser = input switch
        // {
        //     RegisterClientGoogleDTO newClient => unitOfWork.Clients.CreateClient(newClient),
        //     RegisterBroadcasterGoogleDTO newBroadcaster => unitOfWork.Broadcasters.CreateBroadcaster(newBroadcaster),
        //     _ => throw new ArgumentException("Tipo de registro inválido.")
        // };

        await unitOfWork.SaveChangesAsync();

        return new AuthPayload(null, UserMapper.ToDTO(newUser), null);
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
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

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
using Application.Common;
using Domain.Common.Errors;

namespace Application.Services;

public class AuthService(IHasher hasher, IUnitOfWork unitOfWork, IConfiguration configuration, IGoogleAuthService googleAuthService) : IAuthService
{
    private readonly IConfiguration _config = configuration;

    public async Task<ResultAPI<AuthPayload>> RegisterBroadcasterAsync(RegisterBroadcasterInput input)
    {
        // Validar datos básicos antes de consultar BD
        var validationResult = ValidateRegisterInput(input);
        if (validationResult.IsFailure) return ResultAPI<AuthPayload>.BadRequest(validationResult);

        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1);

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return ResultAPI<AuthPayload>.Conflict("El RUT ya está registrado.");
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return ResultAPI<AuthPayload>.Conflict("El email ya está registrado.");
        }

        if (country is null) return ResultAPI<AuthPayload>.NotFound($"Country with code {input.CountryCode} not found.");
        if (category is null) return ResultAPI<AuthPayload>.NotFound($"Category with id {1} not found.");


        Result<Broadcaster, AppError> result = Broadcaster.SignUp(input, country, category, hasher.Hash(input.Password));

        if (result.IsFailure) return ResultAPI<AuthPayload>.BadRequest(result);

        if (result.Value is not Broadcaster broadcaster)
            return ResultAPI<AuthPayload>.Internal("User is not a broadcaster");

        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();

        return ResultAPI<AuthPayload>.Success(new AuthPayload(GenerateJWT(broadcaster), broadcaster));
    }

    public async Task<ResultAPI<AuthPayload>> RegisterClientAsync(RegisterClientInput input)
    {
        // Validar datos básicos antes de consultar BD
        var validationResult = ValidateRegisterInput(input);
        if (validationResult.IsFailure) return ResultAPI<AuthPayload>.BadRequest(validationResult);

        // Validar agencia
        if (string.IsNullOrEmpty(input.AgencyName) || input.AgencyName.Length < 3)
            return ResultAPI<AuthPayload>.BadRequest("AgencyName must be at least 3 characters long");
        if (input.AgencyName.Length > 100)
            return ResultAPI<AuthPayload>.BadRequest("AgencyName must be at most 100 characters long");
        if (!input.AgencyName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            return ResultAPI<AuthPayload>.BadRequest("AgencyName must contain only letters");

        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= new Agency(input.AgencyName);

        if (country is null) return ResultAPI<AuthPayload>.NotFound($"Country with code {input.CountryCode} not found.");

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return ResultAPI<AuthPayload>.Conflict("El RUT ya está registrado.");
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return ResultAPI<AuthPayload>.Conflict("El email ya está registrado.");
        }

        Result<Client, AppError> result = Client.SignUp(input, country, agency, hasher.Hash(input.Password));

        if (result.IsFailure) return ResultAPI<AuthPayload>.BadRequest(result);

        if (result.Value is not Client client)
            return ResultAPI<AuthPayload>.Internal("User is not a client");

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();

        return ResultAPI<AuthPayload>.Success(new AuthPayload(GenerateJWT(client), client));
    }

    public async Task<ResultAPI<AuthPayload>> LoginAsync(UserLoginInput input)
    {
        User user = await unitOfWork.Users.GetUserByEmailAsync(input.Email) ?? throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

        if (user.Password is null) return ResultAPI<AuthPayload>.NotFound("El usuario deberia ingresar con su cuenta de Google.");
        if (!hasher.Verify(input.Password, user.Password)) return ResultAPI<AuthPayload>.NotFound("Email o contraseña incorrectos.");

        return ResultAPI<AuthPayload>.Success(new AuthPayload(GenerateJWT(user), user));
    }

    public async Task<ResultAPI<GoogleAuthPayload>> GoogleAuthAsync(GoogleAuthInput input)
    {
        // Validar token con Google
        GoogleUserInfo? payload = await googleAuthService.ExchangeCodeAsync(input.Code);

        if (payload is null) return ResultAPI<GoogleAuthPayload>.NotFound("Token de Google inválido.");

        User? user = await unitOfWork.Users.GetUserByGoogleIdAsync(payload.Subject);

        // Vincula GoogleId si entró antes con email / password
        if (user is not null && user.GoogleId is null)
        {
            user.GoogleId = payload.Subject;
            await unitOfWork.SaveChangesAsync();
        }

        return ResultAPI<GoogleAuthPayload>.Success(new GoogleAuthPayload
        {
            Token = user is not null ? GenerateJWT(user) : null,
            RequiresRegistration = user is null,
            Subject = payload.Subject,
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
        });
    }

    public async Task<ResultAPI<AuthPayload>> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1);

        if (country is null) return ResultAPI<AuthPayload>.NotFound($"Country with code {input.CountryCode} not found.");
        if (category is null) return ResultAPI<AuthPayload>.NotFound($"Category with id {1} not found.");

        Result<Broadcaster, AppError> result = Broadcaster.SignUpFromGoogle(input, country, category);
        if (result.IsFailure) return ResultAPI<AuthPayload>.BadRequest(result);

        if (result.Value is not Broadcaster broadcaster)
            return ResultAPI<AuthPayload>.Internal("User is not a broadcaster");

        unitOfWork.Broadcasters.CreateBroadcaster(broadcaster);
        await unitOfWork.SaveChangesAsync();

        return ResultAPI<AuthPayload>.Success(new AuthPayload(GenerateJWT(broadcaster), broadcaster));
    }

    public async Task<ResultAPI<AuthPayload>> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= new Agency(input.AgencyName);

        if (country is null) return ResultAPI<AuthPayload>.NotFound($"Country with code {input.CountryCode} not found.");

        Result<Client, AppError> result = Client.SignUpFromGoogle(input, country, agency);
        if (result.IsFailure) return ResultAPI<AuthPayload>.BadRequest(result);

        if (result.Value is not Client client)
            return ResultAPI<AuthPayload>.Internal("User is not a client");

        unitOfWork.Clients.CreateClient(client);
        await unitOfWork.SaveChangesAsync();

        return ResultAPI<AuthPayload>.Success(new AuthPayload(GenerateJWT(client), client));
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

    private Result<AppError> ValidateRegisterInput(RegisterUserInput input)
    {
        return Result<AppError>.Combine(
            ValidateEmail(input.Email),
            ValidatePassword(input.Password),
            ValidateFirstName(input.FirstName),
            ValidateLastName(input.LastName),
            ValidateRUT(input.RUT),
            ValidateCity(input.City),
            ValidateState(input.State),
            ValidateStreet(input.Street)
        );
    }

    private Result<AppError> ValidateEmail(string? email)
    {
        if (email == null) return Result<AppError>.Failure(AppError.Validation("Email is required"));
        if (!email.Contains('@')) return Result<AppError>.Failure(AppError.Validation("Email is missing '@' character"));
        if (email.Length < 10) return Result<AppError>.Failure(AppError.Validation("Email must be at least 10 characters long"));
        if (email.Length > 100) return Result<AppError>.Failure(AppError.Validation("Email must be at most 100 characters long"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidatePassword(string? password)
    {
        if (password == null) return Result<AppError>.Failure(AppError.Validation("Password is required"));
        if (password.Length < 10) return Result<AppError>.Failure(AppError.Validation("Password must be at least 10 characters long"));
        if (password.Length > 60) return Result<AppError>.Failure(AppError.Validation("Password must be at most 60 characters long"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidateFirstName(string? firstName)
    {
        if (firstName == null) return Result<AppError>.Failure(AppError.Validation("FirstName is required"));
        if (firstName.Length < 3) return Result<AppError>.Failure(AppError.Validation("FirstName must be at least 3 characters long"));
        if (firstName.Length > 50) return Result<AppError>.Failure(AppError.Validation("FirstName must be at most 50 characters long"));
        if (!firstName.All(char.IsLetter)) return Result<AppError>.Failure(AppError.Validation("FirstName must contain only letters"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidateLastName(string? lastName)
    {
        if (lastName == null) return Result<AppError>.Failure(AppError.Validation("LastName is required"));
        if (lastName.Length < 3) return Result<AppError>.Failure(AppError.Validation("LastName must be at least 3 characters long"));
        if (lastName.Length > 50) return Result<AppError>.Failure(AppError.Validation("LastName must be at most 50 characters long"));
        if (!lastName.All(char.IsLetter)) return Result<AppError>.Failure(AppError.Validation("LastName must contain only letters"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidateRUT(string? rut)
    {
        if (rut == null) return Result<AppError>.Failure(AppError.Validation("RUT is required"));
        if (rut.Length != 12) return Result<AppError>.Failure(AppError.Validation("RUT must be exactly 12 characters long"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidateCity(string? city)
    {
        if (city == null) return Result<AppError>.Failure(AppError.Validation("City is required"));
        if (city.Length < 4) return Result<AppError>.Failure(AppError.Validation("City must be at least 4 characters long"));
        if (city.Length > 100) return Result<AppError>.Failure(AppError.Validation("City must be at most 100 characters long"));
        if (!city.All(char.IsLetter)) return Result<AppError>.Failure(AppError.Validation("City must contain only letters"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidateState(string? state)
    {
        if (state == null) return Result<AppError>.Failure(AppError.Validation("State is required"));
        if (state.Length < 4) return Result<AppError>.Failure(AppError.Validation("State must be at least 4 characters long"));
        if (state.Length > 100) return Result<AppError>.Failure(AppError.Validation("State must be at most 100 characters long"));
        if (!state.All(char.IsLetter)) return Result<AppError>.Failure(AppError.Validation("State must contain only letters"));
        return Result<AppError>.Success();
    }

    private Result<AppError> ValidateStreet(string? street)
    {
        if (street == null) return Result<AppError>.Success();
        if (street.Length < 4) return Result<AppError>.Failure(AppError.Validation("Street must be at least 4 characters long"));
        if (street.Length > 100) return Result<AppError>.Failure(AppError.Validation("Street must be at most 100 characters long"));
        return Result<AppError>.Success();
    }
}

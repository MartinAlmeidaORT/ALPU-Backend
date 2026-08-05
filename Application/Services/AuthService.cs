using Domain.Interfaces.Public.Repositories;
using Domain.Common;
using Domain.Models;
using Domain.Interfaces.Private;
using Domain.Common.Inputs.Auth;
using Domain.Common.Payloads;
using FluentResults;
using Domain.Interfaces.Public.Services;

namespace Application.Services;

public class AuthService(
    IHasher hasher,
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    IGoogleAuthService googleAuthService,
    IEmailService emailService) : IAuthService
{
    private readonly IJwtService _jwtService = jwtService;

    private readonly IEmailService _emailService = emailService;

    public async Task<Result<AuthPayload>> RegisterBroadcasterAsync(RegisterBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Department? department = await unitOfWork.Departments.GetByIdAsync(input.DepartmentId);

        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1)
        ?? throw new ArgumentNullException($"La categoria de locutor {1} no existe en la base de datos.");

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return UserErrors.DuplicatedRut(input.RUT);
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return UserErrors.DuplicatedEmail(input.Email);
        }

        if (input.IdentityCard != null && unitOfWork.Users.GetAllUsers().Any(u => u.IdentityCard == input.IdentityCard))
        {
            return UserErrors.DuplicatedIdentityCard(input.IdentityCard);
        }

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);
        if (department is null) return DepartmentErrors.DepartmentNotFound(input.DepartmentId);

        Result<Broadcaster> result = Broadcaster.SignUp(input, country, department, category, hasher.Hash(input.Password));

        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Broadcasters.CreateBroadcaster(result.Value);
        await unitOfWork.SaveChangesAsync();
        await _emailService.SendAccountPendingAsync(result.Value.Email, result.Value.FullName);

        return Result.Ok(new AuthPayload(_jwtService.GenerateJWT(result.Value), result.Value));
    }

    public async Task<Result<AuthPayload>> RegisterClientAsync(RegisterClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Department? department = await unitOfWork.Departments.GetByIdAsync(input.DepartmentId);
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= new Agency(input.AgencyName);

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return UserErrors.DuplicatedRut(input.RUT);
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return UserErrors.DuplicatedEmail(input.Email);
        }

        if (input.IdentityCard != null && unitOfWork.Users.GetAllUsers().Any(u => u.IdentityCard == input.IdentityCard))
        {
            return UserErrors.DuplicatedIdentityCard(input.IdentityCard);
        }

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);
        if (department is null) return DepartmentErrors.DepartmentNotFound(input.DepartmentId);

        Result<Client> result = Client.SignUp(input, country, department, agency, hasher.Hash(input.Password));

        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Clients.CreateClient(result.Value);
        await unitOfWork.SaveChangesAsync();
        await _emailService.SendAccountPendingAsync(result.Value.Email, result.Value.FullName);

        return Result.Ok(new AuthPayload(_jwtService.GenerateJWT(result.Value), result.Value));
    }

    public async Task<Result<AuthPayload>> LoginAsync(UserLoginInput input)
    {
        User? user = await unitOfWork.Users.GetUserByEmailAsync(input.Email);

        if (user is null) return UserErrors.LoginFailed();

        if (user.Password is null) return UserErrors.GoogleUserTryNormalLogin();

        if (!hasher.Verify(input.Password, user.Password)) return UserErrors.LoginFailed();

        return Result.Ok(new AuthPayload(_jwtService.GenerateJWT(user), user));
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
            Token = user is not null ? _jwtService.GenerateJWT(user) : null,
            RequiresRegistration = user is null,
            Subject = payload.Subject,
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            User = user
        });
    }

    public async Task<Result<AuthPayload>> CompleteGoogleSignUpBroadcasterAsync(CompleteGoogleSignUpBroadcasterInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Department? department = await unitOfWork.Departments.GetByIdAsync(input.DepartmentId);

        BroadcasterCategory? category = await unitOfWork.Broadcasters.GetCategoryByIdAsync(1)
        ?? throw new ArgumentNullException($"La categoria de locutor {1} no existe en la base de datos.");

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);
        if (department is null) return DepartmentErrors.DepartmentNotFound(input.DepartmentId);

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return UserErrors.DuplicatedRut(input.RUT);
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return UserErrors.DuplicatedEmail(input.Email);
        }

        if (input.IdentityCard != null && unitOfWork.Users.GetAllUsers().Any(u => u.IdentityCard == input.IdentityCard))
        {
            return UserErrors.DuplicatedIdentityCard(input.IdentityCard);
        }

        Result<Broadcaster> result = Broadcaster.SignUpFromGoogle(input, country, department, category);
        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Broadcasters.CreateBroadcaster(result.Value);
        await unitOfWork.SaveChangesAsync();
        await _emailService.SendAccountPendingAsync(result.Value.Email, result.Value.FullName);

        return Result.Ok(new AuthPayload(_jwtService.GenerateJWT(result.Value), result.Value));
    }

    public async Task<Result<AuthPayload>> CompleteGoogleSignUpClientAsync(CompleteGoogleSignUpClientInput input)
    {
        Country? country = await unitOfWork.Countries.GetByCodeAsync(input.CountryCode);
        Department? department = await unitOfWork.Departments.GetByIdAsync(input.DepartmentId);
        Agency? agency = await unitOfWork.Clients.GetAgencyByNameAsync(input.AgencyName);
        agency ??= new Agency(input.AgencyName);

        if (country is null) return CountryErrors.CountryNotFound(input.CountryCode);
        if (department is null) return DepartmentErrors.DepartmentNotFound(input.DepartmentId);

        if (await unitOfWork.Users.GetUserByRutAsync(input.RUT) != null)
        {
            return UserErrors.DuplicatedRut(input.RUT);
        }

        if (await unitOfWork.Users.GetUserByEmailAsync(input.Email) != null)
        {
            return UserErrors.DuplicatedEmail(input.Email);
        }

        if (input.IdentityCard != null && unitOfWork.Users.GetAllUsers().Any(u => u.IdentityCard == input.IdentityCard))
        {
            return UserErrors.DuplicatedIdentityCard(input.IdentityCard);
        }

        Result<Client> result = Client.SignUpFromGoogle(input, country, department, agency);
        if (result.IsFailed) return result.ToResult<AuthPayload>();

        unitOfWork.Clients.CreateClient(result.Value);
        await unitOfWork.SaveChangesAsync();
        await _emailService.SendAccountPendingAsync(result.Value.Email, result.Value.FullName);

        return Result.Ok(new AuthPayload(_jwtService.GenerateJWT(result.Value), result.Value));
    }
}

using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using Domain.Enums;
using FluentResults;

namespace Domain.Models;

public abstract class User : Entity
{
    protected User() { }

    protected User(RegisterUserInput input, Country country, Department department)
    {
        Email = input.Email;
        Password = input.Password;
        FirstName = input.FirstName;
        LastName = input.LastName;
        RUT = input.RUT;
        Address = new Address(country, department, input.City, input.Street);
    }

    protected User(CompleteGoogleSignUpUserInput input, Country country, Department department)
    {
        GoogleId = input.Subject;
        Email = input.Email;
        Password = null;
        FirstName = input.FirstName;
        LastName = input.LastName;
        RUT = input.RUT;
        Address = new Address(country, department, input.City, input.Street);
    }

    public void Update(UpdateUserInput input, Country? country, Department? department)
    {
        Email = input.Email ?? Email;
        FirstName = input.FirstName ?? FirstName;
        LastName = input.LastName ?? LastName;
        RUT = input.RUT ?? RUT;
        Address.Update(country, department, input.Address);
    }

    public virtual Result ValidateSignUp()
    {
        return Result.Merge(
            ValidateEmail(),
            ValidatePassword(),
            ValidateFirstName(),
            ValidateLastName(),
            ValidateRUT(),
            Address.ValidateAddress()
        );
    }

    public virtual Result ValidateGoogleSignUp()
    {
        if (GoogleId == null) return UserErrors.GoogleIdIsRequired();

        return Result.Merge(
            ValidateFirstName(),
            ValidateLastName(),
            ValidateRUT(),
            Address.ValidateAddress()
        );
    }

    public Result ValidateEmail()
    {
        if (Email == null) return UserErrors.EmailIsRequired();

        Result errors = new();

        if (!Email.Contains('@'))
        {
            errors.WithError(UserErrors.EmailIsMissingAtCharacter(Email));
        }

        if (Email.Length < 10)
        {
            errors.WithError(UserErrors.EmailMinLength(Email));
        }

        if (Email.Length > 100)
        {
            errors.WithError(UserErrors.EmailMaxLength(Email));
        }

        if (errors.IsFailed)
        {
            return errors;
        }
        else
        {
            return Result.Ok();
        }
    }

    public Result ValidatePassword()
    {
        if (Password == null) return UserErrors.PasswordIsRequired();

        Result errors = new();

        if (Password.Length < 10)
        {
            errors.WithError(UserErrors.PasswordMinLength());
        }

        if (Password.Length > 60)
        {
            errors.WithError(UserErrors.PasswordMaxLength());
        }

        if (errors.IsFailed)
        {
            return errors;
        }
        else
        {
            return Result.Ok();
        }
    }

    public Result ValidateFirstName()
    {
        if (FirstName == null) return UserErrors.FirstNameIsRequired();

        Result errors = new();

        if (FirstName.Length < 3)
        {
            errors.WithError(UserErrors.FirstNameMaxLength());
        }

        if (FirstName.Length > 50)
        {
            errors.WithError(UserErrors.FirstNameMaxLength());
        }

        if (!FirstName.All(char.IsLetter))
        {
            errors.WithError(UserErrors.FirstNameLettersOnly());
        }

        if (errors.IsFailed)
        {
            return errors;
        }
        else
        {
            return Result.Ok();
        }
    }

    public Result ValidateLastName()
    {
        if (LastName == null) return UserErrors.LastNameIsRequired();

        Result errors = new();

        if (LastName.Length < 3)
        {
            errors.WithError(UserErrors.LastNameMinLength());
        }

        if (LastName.Length > 50)
        {
            errors.WithError(UserErrors.LastNameMaxLength());
        }

        if (!LastName.All(char.IsLetter))
        {
            errors.WithError(UserErrors.LastNameLettersOnly());
        }

        if (errors.IsFailed)
        {
            return errors;
        }
        else
        {
            return Result.Ok();
        }
    }

    public Result ValidateRUT()
    {
        if (RUT == null) return UserErrors.RUTIsRequired();

        if (RUT.Length != 12) return UserErrors.RUTIsInvalid(RUT);

        return Result.Ok();
    }

    public int UserId { get; set; }

    public string? GoogleId { get; set; } = null!;

    public UserState UserState { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string RUT { get; set; } = null!;

    public int AddressId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = [];
}

public static class UserErrors
{
    // Errors class
    public class UserNotFoundError(string msg) : NotFoundError(msg);

    public class GoogleUserTryNormalLoginError(string msg) : BadRequestError(msg);
    public class GoogleTokenIsInvalidError(string msg) : AuthError(msg);
    public class GoogleIdIsRequiredError(string msg) : BadRequestError(msg);

    public class EmailIsRequiredError(string msg) : ValidationError(msg);
    public class EmailIsMissingAtCharacterError(string msg) : ValidationError(msg);
    public class EmailMinLengthError(string msg) : ValidationError(msg);
    public class EmailMaxLengthError(string msg) : ValidationError(msg);
    public class DuplicatedEmailError(string msg) : ConflictError(msg);

    public class RUTIsRequiredError(string msg) : ConflictError(msg);
    public class RUTIsInvalidError(string msg) : ValidationError(msg);
    public class DuplicatedRUTError(string msg) : ConflictError(msg);

    public class PasswordIsRequiredError(string msg) : ValidationError(msg);
    public class PasswordMinLengthError(string msg) : ValidationError(msg);
    public class PasswordMaxLengthError(string msg) : ValidationError(msg);

    public class FirstNameIsRequiredError(string msg) : ValidationError(msg);
    public class FirstNameMinLengthError(string msg) : ValidationError(msg);
    public class FirstNameMaxLengthError(string msg) : ValidationError(msg);
    public class FirstNameLettersOnlyError(string msg) : ValidationError(msg);

    public class LastNameIsRequiredError(string msg) : ValidationError(msg);
    public class LastNameMinLengthError(string msg) : ValidationError(msg);
    public class LastNameMaxLengthError(string msg) : ValidationError(msg);
    public class LastNameLettersOnlyError(string msg) : ValidationError(msg);

    // Factory Methods
    public static UserNotFoundError UserNotFound(int id) => new($"Usuario con {id} no encontrado.");
    public static AuthError LoginFailed() => new("Email o contraseña incorrectos.");

    public static GoogleUserTryNormalLoginError GoogleUserTryNormalLogin() => new("El usuario debe ingresar con su cuenta de Google.");
    public static GoogleTokenIsInvalidError GoogleTokenIsInvalid() => new("El token de Google es inválido.");
    public static GoogleIdIsRequiredError GoogleIdIsRequired() => new("Google ID es requerido.");

    public static EmailIsRequiredError EmailIsRequired() => new("Email es requerido.");
    public static EmailIsMissingAtCharacterError EmailIsMissingAtCharacter(string email) => new($"Email is missing the '@' character. {email}");
    public static EmailMinLengthError EmailMinLength(string email) => new($"Email must be at least 10 characters long. {email}");
    public static EmailMaxLengthError EmailMaxLength(string email) => new($"Email must be at most 100 characters long. {email}");
    public static DuplicatedEmailError DuplicatedEmail(string email) => new($"Email {email} ya esta en uso.");

    public static RUTIsRequiredError RUTIsRequired() => new("RUT es requerido.");
    public static RUTIsInvalidError RUTIsInvalid(string rut) => new($"RUT debe tener exactamente 12 caracteres. {rut}");
    public static DuplicatedRUTError DuplicatedRUT(string rut) => new($"RUT {rut} ya esta en uso.");

    public static PasswordIsRequiredError PasswordIsRequired() => new("Se requiere de una contraseña.");
    public static PasswordMinLengthError PasswordMinLength() => new("La contraseña debe tener al menos 10 caracteres.");
    public static PasswordMaxLengthError PasswordMaxLength() => new("La contraseña puede tener un maximo de 60 caracteres.");

    public static FirstNameIsRequiredError FirstNameIsRequired() => new("El nombre no es opcional.");
    public static FirstNameMinLengthError FirstNameMinLength() => new("El nombre debe tener al menos 3 caracteres.");
    public static FirstNameMaxLengthError FirstNameMaxLength() => new("El nombre puede tener hasta 50 caracteres.");
    public static FirstNameLettersOnlyError FirstNameLettersOnly() => new("El nombre solo puede tener letras.");

    public static LastNameIsRequiredError LastNameIsRequired() => new("El apellido no es opcional.");
    public static LastNameMinLengthError LastNameMinLength() => new("El apellido debe tener al menos 3 caracteres.");
    public static LastNameMaxLengthError LastNameMaxLength() => new("El apellido puede tener hasta 50 caracteres.");
    public static LastNameLettersOnlyError LastNameLettersOnly() => new("El apellido solo puede tener letras.");
}

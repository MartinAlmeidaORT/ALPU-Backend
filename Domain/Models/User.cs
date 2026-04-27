using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[Table("user")]
[Index("Email", Name = "user_email_key", IsUnique = true)]
[Index("RUT", Name = "user_rut_key", IsUnique = true)]
[Index("GoogleId", Name = "user_google_id_key", IsUnique = true)]
public abstract class User : Entity
{
    protected User() { }

    protected User(RegisterUserInput input, Country country)
    {
        Email = input.Email;
        Password = input.Password;
        FirstName = input.FirstName;
        LastName = input.LastName;
        RUT = input.RUT;
        Address = new Address(country, input.State, input.City, input.Street);
    }

    protected User(CompleteGoogleSignUpUserInput input, Country country)
    {
        GoogleId = input.Subject;
        Email = input.Email;
        Password = null;
        FirstName = input.FirstName;
        LastName = input.LastName;
        RUT = input.RUT;
        Address = new Address(country, input.State, input.City, input.Street);
    }

    public void Update(UpdateUserInput input, Country? country)
    {
        Email = input.Email ?? Email;
        FirstName = input.FirstName ?? FirstName;
        LastName = input.LastName ?? LastName;
        RUT = input.RUT ?? RUT;
        Address.Update(country, input.Address);
    }

    public virtual Result<AppError> ValidateSignUp()
    {
        return Result<AppError>.Combine(
            ValidateEmail(),
            ValidatePassword(),
            ValidateFirstName(),
            ValidateLastName(),
            ValidateRUT(),
            Address.ValidateAddress()
        );
    }

    public virtual Result<AppError> ValidateGoogleSignUp()
    {
        if (GoogleId == null) return Result<AppError>.Failure(AppError.Validation("GoogleId is required"));

        return Result<AppError>.Combine(
            ValidateFirstName(),
            ValidateLastName(),
            ValidateRUT(),
            Address.ValidateAddress()
        );
    }

    public Result<AppError> ValidateEmail()
    {
        if (Email == null) return Result<AppError>.Failure(AppError.Validation("Email is required"));

        return Result<AppError>.Combine(
            Require(Email.Contains('@'), "Email is missing '@' character"),
            Require(Email.Length >= 10, "Email must be at least 10 characters long"),
            Require(Email.Length <= 100, "Email must be at most 100 characters long")
        );
    }

    public Result<AppError> ValidatePassword()
    {
        if (Password == null) return Result<AppError>.Failure(AppError.Validation("Password is required"));

        return Result<AppError>.Combine(
            Require(Password.Length >= 10, "Password must be at least 10 characters long"),
            Require(Password.Length <= 60, "Password must be at most 60 characters long")
        );
    }

    public Result<AppError> ValidateFirstName()
    {
        if (FirstName == null) return Result<AppError>.Failure(AppError.Validation("FirstName is required"));

        return Result<AppError>.Combine(
            Require(FirstName.Length >= 3, "FirstName must be at least 3 characters long"),
            Require(FirstName.Length <= 50, "FirstName must be at most 50 characters long"),
            Require(FirstName.All(char.IsLetter), "FirstName must contain only letters")
        );
    }

    public Result<AppError> ValidateLastName()
    {
        if (LastName == null) return Result<AppError>.Failure(AppError.Validation("LastName is required"));

        return Result<AppError>.Combine(
            Require(LastName.Length >= 3, "LastName must be at least 3 characters long"),
            Require(LastName.Length <= 50, "LastName must be at most 50 characters long"),
            Require(LastName.All(char.IsLetter), "LastName must contain only letters")
        );
    }

    public Result<AppError> ValidateRUT()
    {
        if (RUT == null) return Result<AppError>.Failure(AppError.Validation("RUT is required"));

        return Require(RUT.Length == 12, "RUT must be exactly 12 characters long");
    }

    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("google_id")]
    public string GoogleId { get; set; } = null!;

    [Column("state", TypeName = "user_state_enum")]
    [EnumDataType(typeof(UserState))]
    public UserState UserState { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("password")]
    [StringLength(50)]
    public string? Password { get; set; }

    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("rut")]
    [StringLength(12)]
    public string RUT { get; set; } = null!;

    [Column("address_id")]
    public int AddressId { get; set; }

    [ForeignKey("AddressId")]
    public virtual Address Address { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = [];
}

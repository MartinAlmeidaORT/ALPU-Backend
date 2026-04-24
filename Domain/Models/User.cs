using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

[Table("user")]
[Index("Email", Name = "user_email_key", IsUnique = true)]
[Index("RUT", Name = "user_rut_key", IsUnique = true)]
[Index("GoogleId", Name = "user_google_id_key", IsUnique = true)]
public partial class User : Entity
{
    public User() { }

    public User(RegisterUserInput input, Country country)
    {
        Email = input.Email;
        FirstName = input.FirstName;
        LastName = input.LastName;
        RUT = input.RUT;
        Address = new Address(country, input.State, input.City, input.Street);
    }

    public User(CompleteGoogleSignUpUserInput input, Country country)
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

using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs.Auth;

namespace Domain.Models;

[Table("client")]
public partial class Client : User
{
    protected Client() { }

    protected Client(RegisterClientInput input, Country country, Agency agency)
        : base(input, country)
    {
        AgencyId = agency.AgencyId;
        Agency = agency;
    }

    protected Client(CompleteGoogleSignUpClientInput input, Country country, Agency agency)
        : base(input, country)
    {
        AgencyId = agency.AgencyId;
        Agency = agency;
    }

    public static Result<Client, AppError> SignUp(RegisterClientInput input, Country country, Agency agency, string passwordHashed)
    {
        Client newClient = new(input, country, agency);
        Result<AppError> result = newClient.ValidateSignUp();
        if (!result.IsSuccess) return Result<Client, AppError>.Failure(result.Errors);
        newClient.Password = passwordHashed;
        return Result<Client, AppError>.Success(newClient);
    }

    public static Result<Client, AppError> SignUpFromGoogle(CompleteGoogleSignUpClientInput input, Country country, Agency agency)
    {
        Client newClient = new(input, country, agency);
        Result<AppError> result = newClient.ValidateGoogleSignUp();
        if (!result.IsSuccess) return Result<Client, AppError>.Failure(result.Errors);
        return Result<Client, AppError>.Success(newClient);
    }

    [Column("agency_id")]
    public int AgencyId { get; set; }

    [ForeignKey("AgencyId")]
    [InverseProperty("Clients")]
    public virtual Agency Agency { get; set; } = null!;

    [InverseProperty("Client")]
    public virtual ICollection<Contract> Contracts { get; set; } = [];
}

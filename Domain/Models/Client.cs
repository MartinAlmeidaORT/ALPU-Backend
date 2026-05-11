using Domain.Common.Errors;
using Domain.Common.Inputs.Auth;
using FluentResults;

namespace Domain.Models;

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

    public static Result<Client> SignUp(RegisterClientInput input, Country country, Agency agency, string passwordHashed)
    {
        Client newClient = new(input, country, agency);
        Result errors = newClient.ValidateSignUp();
        if (errors.IsFailed) return errors;
        newClient.Password = passwordHashed;
        return newClient;
    }

    public static Result<Client> SignUpFromGoogle(CompleteGoogleSignUpClientInput input, Country country, Agency agency)
    {
        Client newClient = new(input, country, agency);
        Result errors = newClient.ValidateGoogleSignUp();
        if (errors.IsFailed) return errors;
        return newClient;
    }

    public override Result ValidateSignUp()
    {
        return Result.Merge(
            base.ValidateSignUp(),
            ValidateAgency()
        );
    }

    public override Result ValidateGoogleSignUp()
    {
        return Result.Merge(
            base.ValidateGoogleSignUp(),
            ValidateAgency()
        );
    }

    public Result ValidateAgency()
    {
        if (Agency == null)
        {
            return ClientErrors.AgencyIsRequired();
        }
        else
        {
            return Result.Ok();
        }
    }

    public int AgencyId { get; set; }

    public virtual Agency Agency { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = [];
}

public class ClientErrors
{
    public class AgencyIsRequiredError(string msg) : ValidationError(msg);

    public static AgencyIsRequiredError AgencyIsRequired() => new($"Debe registrar una agencia.");
}

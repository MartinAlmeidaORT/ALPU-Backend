using Domain.Common.Abstracts;
using Domain.Common.Errors;
using FluentResults;

namespace Domain.Models;

public class Agency : Entity
{
    internal Agency() { }

    public Agency(string name)
    {
        Name = name;
    }

    public int AgencyId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = [];

    public Result Validate()
    {
        if (Name is null) return AgencyErrors.NameIsRequired();

        Result errors = new();

        if (Name.Length < 3)
        {
            errors.WithError(AgencyErrors.NameMinLength(Name));
        }

        if (Name.Length > 100)
        {
            errors.WithError(AgencyErrors.NameMaxLength(Name));
        }

        if (!Name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
        {
            errors.WithError(AgencyErrors.NameIsLettersOnly(Name));
        }

        if (errors.IsSuccess)
        {
            return Result.Ok();
        }
        else
        {
            return errors;
        }
    }
}

public class AgencyErrors
{
    public class NameIsRequiredError(string msg) : ValidationError(msg);
    public class NameMinLengthError(string msg) : ValidationError(msg);
    public class NameMaxLengthError(string msg) : ValidationError(msg);
    public class NameIsLettersOnlyError(string msg) : ValidationError(msg);

    public static NameIsRequiredError NameIsRequired() => new($"Es necesario un nombre para la agencia.");
    public static NameMinLengthError NameMinLength(string name) => new($"El nombre debe tener por lo menos 4 caracteres. {name}");
    public static NameMaxLengthError NameMaxLength(string name) => new($"El nombre debe tener menos de 100 caracteres. {name}");
    public static NameIsLettersOnlyError NameIsLettersOnly(string name) => new($"El nombre solo puede tener letras. {name}");
}

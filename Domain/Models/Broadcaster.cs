using System.Text.RegularExpressions;
using Domain.Common.Errors;
using Domain.Common.Inputs;
using Domain.Common.Inputs.Auth;
using FluentResults;

namespace Domain.Models;

public partial class Broadcaster : User
{
    internal Broadcaster() { }

    protected Broadcaster(RegisterBroadcasterInput input, Country country, Department department, BroadcasterCategory category)
        : base(input, country, department)
    {
        CategoryId = category.BroadcasterCategoryId;
        Category = category;
    }

    protected Broadcaster(CompleteGoogleSignUpBroadcasterInput input, Country country, Department department, BroadcasterCategory category)
        : base(input, country, department)
    {
        CategoryId = category.BroadcasterCategoryId;
        Category = category;
    }

    public static Result<Broadcaster> SignUp(RegisterBroadcasterInput input, Country country, Department department, BroadcasterCategory category, string passwordHashed)
    {
        Broadcaster newBroadcaster = new(input, country, department, category);
        Result errors = newBroadcaster.ValidateSignUp();
        if (errors.IsFailed) return errors;
        newBroadcaster.Password = passwordHashed;
        return newBroadcaster;
    }

    public static Result<Broadcaster> SignUpFromGoogle(CompleteGoogleSignUpBroadcasterInput input, Country country, Department department, BroadcasterCategory category)
    {
        Broadcaster newBroadcaster = new(input, country, department, category);
        Result errors = newBroadcaster.ValidateGoogleSignUp();
        if (errors.IsFailed) return errors;
        return newBroadcaster;
    }

    public int CategoryId { get; set; }

    public int MaxDemos { get; } = 5;

    public BroadcasterCategory Category { get; set; } = null!;

    [GeneratedRegex(@"^\+?[0-9]+$")]
    private static partial Regex PhoneRegex();

    public string? PhoneNumber { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public ICollection<Contract> Contracts { get; set; } = [];

    public ICollection<Demo> Demos { get; set; } = [];

    public ICollection<Membership> Memberships { get; set; } = [];

    public ICollection<Skill> Skills { get; set; } = [];

    public ICollection<Language> Languages { get; set; } = [];

    public Result<Demo> AddDemo(string fileKey, Language language, string title)
    {
        Result<Demo> result = Demo.CreateDemo(UserId, fileKey, language, title);
        if (result.IsFailed) return result;

        if (Demos.Count >= MaxDemos)
        {
            return Result.Fail(DemoErrors.MaxDemosReached(MaxDemos));
        }

        Demos.Add(result.Value);
        return result;
    }

    public Result<Demo> RemoveDemo(string key)
    {
        Demo? demo = Demos.SingleOrDefault(d => d.FileName == key);
        if (demo == null)
        {
            return Result.Fail(DemoErrors.DemoNotFound(key));
        }

        Demos.Remove(demo);
        return Result.Ok(demo);
    }

    public Result Update(UpdateUserInput input, Country? country, Department? department, string? phoneNumber, string? website, string? description)
    {
        base.Update(input, country, department);
        PhoneNumber = phoneNumber ?? PhoneNumber;
        Website = website ?? Website;
        Description = description ?? Description;
        return ValidateUpdate();
    }

    public override Result ValidateUpdate()
    {
        Result baseResult = base.ValidateUpdate();
        return Result.Merge(
            baseResult,
            ValidatePhoneNumber(),
            ValidateWebsite(),
            ValidateDescription()
        );
    }

    public Result ValidatePhoneNumber()
    {
        if (PhoneNumber == null) return Result.Ok();

        if (PhoneNumber.Length > 15)
        {
            return Result.Fail(BroadcasterErrors.PhoneNumberMaxLength());
        }

        if (!PhoneRegex().IsMatch(PhoneNumber))
        {
            return Result.Fail(BroadcasterErrors.InvalidNumberFormat());
        }

        return Result.Ok();
    }

    public Result ValidateWebsite()
    {
        if (Website == null) return Result.Ok();

        if (!Uri.TryCreate(Website, UriKind.Absolute, out Uri? uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return Result.Fail(BroadcasterErrors.InvalidWebsite());
        }

        return Result.Ok();
    }

    public Result ValidateDescription()
    {
        if (Description == null) return Result.Ok();

        if (Description.Length < 50)
        {
            return Result.Fail(BroadcasterErrors.DescriptionMinLength());
        }

        if (Description.Length > 250)
        {
            return Result.Fail(BroadcasterErrors.DescriptionMaxLength());
        }

        return Result.Ok();
    }

    public void UpdateSkills(ICollection<Skill> skills) => Skills = skills;

    public void UpdateLanguages(ICollection<Language> languages) => Languages = languages;
}

public static class BroadcasterErrors
{
    // Errors class
    public class PhoneNumberLengthError(string msg) : ValidationError(msg);
    public class PhoneNumbersOnlyError(string msg) : ValidationError(msg);

    public class InvalidWebsiteError(string msg) : ValidationError(msg);

    public class DescriptionMinLengthError(string msg) : ValidationError(msg);
    public class DescriptionMaxLengthError(string msg) : ValidationError(msg);

    // Factory Methods
    public static PhoneNumberLengthError PhoneNumberMaxLength() => new("El telefono debe tener hasta 15 caracteres.");
    public static PhoneNumbersOnlyError InvalidNumberFormat() => new("El telefono solo puede tener numeros.");

    public static InvalidWebsiteError InvalidWebsite() => new("El sitio web no es valido.");

    public static DescriptionMinLengthError DescriptionMinLength() => new("La description debe tener al menos 50 caracteres.");
    public static DescriptionMaxLengthError DescriptionMaxLength() => new("La descripcion puede tener hasta 250 caracteres.");
}

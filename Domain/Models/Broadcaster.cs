using Domain.Common.Inputs.Auth;
using FluentResults;

namespace Domain.Models;

public class Broadcaster : User
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

    public BroadcasterCategory Category { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public ICollection<Contract> Contracts { get; set; } = [];

    public ICollection<Demo> Demos { get; set; } = [];

    public ICollection<Membership> Memberships { get; set; } = [];

    public ICollection<Skill> Skills { get; set; } = [];

    public ICollection<Language> Languages { get; set; } = [];

    public Result<Demo> AddDemo(string fileKey)
    {
        Result<Demo> result = Demo.CreateDemo(UserId, fileKey);
        if (result.IsFailed) return result;

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

    public void UpdateSkills(ICollection<Skill> skills) => Skills = skills;

    public void UpdateLanguages(ICollection<Language> languages) => Languages = languages;

    public void UpdateProfile(string? phoneNumber, string? website, string? description)
    {
        PhoneNumber = phoneNumber ?? PhoneNumber;
        Website = website ?? Website;
        Description = description ?? Description;
    }
}

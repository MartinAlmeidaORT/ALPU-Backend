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

    public virtual BroadcasterCategory Category { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = [];

    public virtual ICollection<Demo> Demos { get; set; } = [];

    public virtual ICollection<Membership> Memberships { get; set; } = [];
}

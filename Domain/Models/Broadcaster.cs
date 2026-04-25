using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.Inputs.Auth;

namespace Domain.Models;

[Table("broadcaster")]
public partial class Broadcaster : User
{
    protected Broadcaster() { }

    protected Broadcaster(RegisterBroadcasterInput input, Country country, BroadcasterCategory category)
        : base(input, country)
    {
        CategoryId = category.BroadcasterCategoryId;
        Category = category;
    }

    protected Broadcaster(CompleteGoogleSignUpBroadcasterInput input, Country country, BroadcasterCategory category)
        : base(input, country)
    {
        CategoryId = category.BroadcasterCategoryId;
        Category = category;
    }

    public static Result<Broadcaster, AppError> SignUp(RegisterBroadcasterInput input, Country country, BroadcasterCategory category, string passwordHashed)
    {
        Broadcaster newBroadcaster = new(input, country, category);
        Result<AppError> result = newBroadcaster.ValidateSignUp();
        if (result.IsFailure) return Result<Broadcaster, AppError>.Failure(result.Errors);
        newBroadcaster.Password = passwordHashed;
        return Result<Broadcaster, AppError>.Success(newBroadcaster);
    }

    public static Result<Broadcaster, AppError> SignUpFromGoogle(CompleteGoogleSignUpBroadcasterInput input, Country country, BroadcasterCategory category)
    {
        Broadcaster newBroadcaster = new(input, country, category);
        Result<AppError> result = newBroadcaster.ValidateGoogleSignUp();
        if (result.IsFailure) return Result<Broadcaster, AppError>.Failure(result.Errors);
        return Result<Broadcaster, AppError>.Success(newBroadcaster);
    }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Broadcasters")]
    public virtual BroadcasterCategory Category { get; set; } = null!;

    [InverseProperty("Broadcaster")]
    public virtual ICollection<Contract> Contracts { get; set; } = [];

    [InverseProperty("Broadcaster")]
    public virtual ICollection<Demo> Demos { get; set; } = [];

    [InverseProperty("Broadcaster")]
    public virtual ICollection<Membership> Memberships { get; set; } = [];
}

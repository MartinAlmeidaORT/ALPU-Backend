using Domain.Enums;

namespace Domain.Common.Inputs;

public record UpdateBroadcasterProfileInput
{
    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? Email { get; init; }

    public Gender? Gender { get; init; }

    public string? IdentityCard { get; init; }

    public UpdateAddressInput? Address { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Website { get; init; }

    public string? Description { get; init; }

    public ICollection<int>? SkillIds { get; init; }

    public ICollection<int>? LanguageIds { get; init; }
}

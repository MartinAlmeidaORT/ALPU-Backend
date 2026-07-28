using Domain.Enums;

namespace Domain.Common.Inputs;

public class UpdateUserInput
{
    public string? IdentityCard { get; set; }

    public Gender? Gender { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? RUT { get; set; }

    public UpdateAddressInput? Address { get; set; }
}

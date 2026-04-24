namespace Domain.Common.Inputs;

public class UpdateUserInput
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RUT { get; set; }
    public UpdateAddressInput? Address { get; set; }
}

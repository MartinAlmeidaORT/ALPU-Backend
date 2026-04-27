namespace Domain.Common.Inputs;

public class UpdateAddressInput
{
    public string? CountryCode { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
}

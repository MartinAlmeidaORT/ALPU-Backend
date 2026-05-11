namespace Domain.Common.Inputs;

public class UpdateAddressInput
{
    public string? CountryCode { get; set; }
    public int? DepartmentId { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
}

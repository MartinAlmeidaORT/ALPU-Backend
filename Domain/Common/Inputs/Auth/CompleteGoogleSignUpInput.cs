namespace Domain.Common.Inputs.Auth;

public abstract record CompleteGoogleSignUpUserInput
{
    public required string Subject { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string RUT { get; init; }
    public required string CountryCode { get; init; }
    public required int DepartmentId { get; init; }
    public required string City { get; init; }
    public string? Street { get; init; }
}

public record CompleteGoogleSignUpBroadcasterInput : CompleteGoogleSignUpUserInput;

public record CompleteGoogleSignUpClientInput : CompleteGoogleSignUpUserInput
{
    public required string AgencyName { get; init; }
}

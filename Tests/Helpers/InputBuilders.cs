using Domain.Common.Inputs.Auth;
using Domain.Enums;

namespace Tests.Helpers;

public static class InputBuilders
{
    public static RegisterBroadcasterInput ValidBroadcasterInput() => new()
    {
        FirstName = "Tadeo",
        LastName = "Mieres",
        Email = "tadeo@alpu.uy",
        Password = "Password123!",
        CountryCode = "UY",
        RUT = "12345678910A",
        DepartmentId = 1,
        City = "Ciudad",
        Gender = Gender.Male,
        IdentityCard = "49933947",
    };

    public static RegisterClientInput ValidClientInput() => new()
    {
        FirstName = "Martin",
        LastName = "Almeida",
        Email = "martin@agency.uy",
        Password = "Password123!",
        CountryCode = "UY",
        AgencyName = "Test Agency",
        RUT = "12345678910A",
        DepartmentId = 1,
        City = "Ciudad",
        Gender = Gender.Male,
        IdentityCard = "49933947",
    };

    public static CompleteGoogleSignUpClientInput ValidGoogleClientInput(string googleId = "google-sub-123") => new()
    {
        FirstName = "Martin",
        LastName = "Almeida",
        Email = "martin@agency.uy",
        CountryCode = "UY",
        AgencyName = "Test Agency",
        Subject = googleId,
        RUT = "12345678910A",
        DepartmentId = 1,
        City = "Ciudad",
        Gender = Gender.Male,
        IdentityCard = "49933947",
    };

    public static CompleteGoogleSignUpBroadcasterInput ValidGoogleBroadcasterInput(string googleId = "google-sub-456") => new()
    {
        FirstName = "Tadeo",
        LastName = "Mieres",
        Email = "tadeo@alpu.uy",
        CountryCode = "UY",
        Subject = googleId,
        RUT = "12345678910A",
        DepartmentId = 1,
        City = "Ciudad",
        Gender = Gender.Male,
        IdentityCard = "49933947",
    };
}

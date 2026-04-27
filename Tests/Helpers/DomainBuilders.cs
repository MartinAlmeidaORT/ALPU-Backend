// Helpers/DomainBuilders.cs
using Domain.Common.Inputs.Auth;
using Domain.Models;

namespace Tests.Helpers;

public static class DomainBuilders
{
    // --- Inputs ---

    public static RegisterBroadcasterInput BroadcasterInput() => new()
    {
        FirstName = "Tadeo",
        LastName = "Mieres",
        Email = "tadeo@alpu.uy",
        Password = "Password123!",
        CountryCode = "UY",
        RUT = "12345678910A",
        State = "Estado",
        City = "Ciudad",
    };
    public static RegisterClientInput ClientInput() => new()
    {
        FirstName = "Martin",
        LastName = "Almeida",
        Email = "martin@agency.uy",
        Password = "Password123!",
        CountryCode = "UY",
        AgencyName = "Test Agency",
        RUT = "12345678910A",
        State = "Estado",
        City = "Ciudad",
    };

    public static CompleteGoogleSignUpClientInput GoogleClientInput(string googleId = "google-sub-123") => new()
    {
        FirstName = "Martin",
        LastName = "Almeida",
        Email = "martin@agency.uy",
        CountryCode = "UY",
        AgencyName = "Test Agency",
        Subject = googleId,
        RUT = "12345678910A",
        State = "Estado",
        City = "Ciudad",
    };

    public static CompleteGoogleSignUpBroadcasterInput GoogleBroadcasterInput(string googleId = "google-sub-456") => new()
    {
        FirstName = "Tadeo",
        LastName = "Mieres",
        Email = "tadeo@alpu.uy",
        CountryCode = "UY",
        Subject = googleId,
        RUT = "12345678910A",
        State = "Estado",
        City = "Ciudad",
    };

    // --- Domain objects ---

    public static Country Country(string code = "UY") => new() { CountryCode = code };
    public static Agency Agency(string name = "Test Agency") => new(name);

    public static Client Client()
    {
        var result = Domain.Models.Client.SignUp(
            ClientInput(),
            Country(),
            Agency(),
            "hashed-password"
        );
        return result.Value!;
    }

    public static Client ClientFromGoogle(string googleId = "google-sub-123")
    {
        var result = Domain.Models.Client.SignUpFromGoogle(
            GoogleClientInput(googleId),
            Country(),
            Agency()
        );
        return result.Value!;
    }

    public static Broadcaster Broadcaster()
    {
        var result = Domain.Models.Broadcaster.SignUp(
            BroadcasterInput(),
            Country(),
            new BroadcasterCategory { BroadcasterCategoryId = 1 },
            "hashed-password"
        );
        return result.Value!;
    }

    public static Broadcaster BroadcasterFromGoogle(string googleId = "google-sub-456")
    {
        var result = Domain.Models.Broadcaster.SignUpFromGoogle(
            GoogleBroadcasterInput(googleId),
            Country(),
            new BroadcasterCategory { BroadcasterCategoryId = 1 }
        );
        return result.Value!;
    }
}

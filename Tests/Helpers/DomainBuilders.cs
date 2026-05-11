using Domain.Models;

namespace Tests.Helpers;

public static class DomainBuilders
{
    public static Country ValidCountry() => new()
    {
        CountryCode = "UY",
        Name = "Uruguay"
    };

    public static Agency ValidAgency() => new("Valid Agency");

    public static Department ValidDepartment() => new()
    {
        DepartmentId = 1,
        Name = "Valid Agency",
        Country = ValidCountry(),
        CountryCode = ValidCountry().CountryCode
    };

    public static Client ValidClient()
    {
        var result = Client.SignUp(
            InputBuilders.ValidClientInput(),
            ValidCountry(),
            ValidDepartment(),
            ValidAgency(),
            "hashed-password"
        );
        return result.Value;
    }

    public static Client ValidClientFromGoogle(string googleId = "google-sub-123")
    {
        var result = Client.SignUpFromGoogle(
            InputBuilders.ValidGoogleClientInput(googleId),
            ValidCountry(),
            ValidDepartment(),
            ValidAgency()
        );
        return result.Value;
    }

    public static Broadcaster ValidBroadcaster()
    {
        var result = Broadcaster.SignUp(
            InputBuilders.ValidBroadcasterInput(),
            ValidCountry(),
            ValidDepartment(),
            new BroadcasterCategory { BroadcasterCategoryId = 1 },
            "hashed-password"
        );
        return result.Value;
    }

    public static Broadcaster ValidBroadcasterFromGoogle(string googleId = "google-sub-456")
    {
        var result = Broadcaster.SignUpFromGoogle(
            InputBuilders.ValidGoogleBroadcasterInput(googleId),
            ValidCountry(),
            ValidDepartment(),
            new BroadcasterCategory { BroadcasterCategoryId = 1 }
        );
        return result.Value;
    }
}

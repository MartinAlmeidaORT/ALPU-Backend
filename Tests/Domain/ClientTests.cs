using Domain.Models;
using FluentAssertions;
using Tests.Helpers;

namespace Tests.Domain;

public class ClientTests
{
    private Client BuildClient()
    {
        var result = Client.SignUp(
            InputBuilders.ValidClientInput(),
            DomainBuilders.ValidCountry(),
            DomainBuilders.ValidDepartment(),
            DomainBuilders.ValidAgency(),
            "hashed"
        );
        return result.Value;
    }

    [Fact]
    public void ValidateAgency_WhenAgencyIsNull_ReturnsFail()
    {
        var client = BuildClient();
        client.Agency = null!;

        var result = client.ValidateSignUp();

        result.IsSuccess.Should().BeFalse();
        result.HasError<ClientErrors.AgencyIsRequiredError>();
    }
}

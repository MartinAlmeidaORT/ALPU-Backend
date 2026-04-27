using System.Net.Http.Json;
using System.Text.Json;
using Domain.Common;
using Domain.Interfaces.Private;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace DataAccess.ExternalServices;

public class GoogleAuthService(HttpClient httpClient, IConfiguration config) : IGoogleAuthService
{
    public async Task<GoogleUserInfo?> ExchangeCodeAsync(string code)
    {
        var response = await httpClient.PostAsync(
            "token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = config["Google:ClientId"]!,
                ["client_secret"] = config["Google:ClientSecret"]!,
                ["redirect_uri"] = config["Google:RedirectUri"]!,
                ["grant_type"] = "authorization_code"
            }));

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var idToken = json.GetProperty("id_token").GetString();


        if (idToken is null) return null;
        return await ValidateGoogleTokenAsync(idToken, config["Google:ClientId"]!);
    }

    private static async Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string token, string clientId)
    {
        try
        {
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [clientId]
                });

            return new GoogleUserInfo
            {
                Subject = payload.Subject,
                Email = payload.Email,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName
            };
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}

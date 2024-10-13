using System;
using System.Text.Json.Serialization;

namespace Timestamp_Backend.Services.Token;

public interface IRefreshTokenServices
{
    public Task<AuthToken?> RefreshToken(string refreshToken);
}

public class RefreshTokenServices(HttpClient httpClient) : IRefreshTokenServices
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<AuthToken?> RefreshToken(string refreshToken)
    {
        var request = new
        {
            grant_type = "refresh_token",
            refreshToken
        };
        var response = await _httpClient.PostAsJsonAsync("", request);
        var token = await response.Content.ReadFromJsonAsync<Token>();
        return token?.ToAuthToken();
    }
}

public record Token
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = null!;
    [JsonPropertyName("user_id")]
    public string IdentityId { get; set; } = null!;
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = null!;

    public AuthToken ToAuthToken()
    {
        return new AuthToken
        {
            IdToken = IdToken,
            IdentityId = IdentityId,
            RefreshToken = RefreshToken,
        };
    }
}
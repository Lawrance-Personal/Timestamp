using System;
using System.Text.Json.Serialization;

namespace Timestamp_Backend.Services.Token;

public interface ITokenServices
{
    public Task<AuthToken?> GenerateToken(string email, string password);
}

public class TokenServices(HttpClient httpClient) : ITokenServices
{
    private readonly HttpClient _httpclient = httpClient;
    public async Task<AuthToken?> GenerateToken(string email, string password)
    {
        var request = new
        {
            Email = email,
            Password = password,
            ReturnSecureToken = true
        };
        var response = await _httpclient.PostAsJsonAsync("", request);
        return await response.Content.ReadFromJsonAsync<AuthToken>();
    }
}

public record AuthToken
{
    [JsonPropertyName("IdToken")]
    public string IdToken { get; set; } = null!;
    [JsonPropertyName("LocalId")]
    public string IdentityId { get; set; } = null!;
    [JsonPropertyName("RefreshToken")]
    public string RefreshToken { get; set; } = null!;
}
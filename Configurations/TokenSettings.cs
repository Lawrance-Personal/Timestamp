using System;

namespace Timestamp_Backend.Configurations;

public class TokenSettings
{
    public string TokenUri { get; set; } = null!;
    public string RefreshTokenUri { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
}

using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Timestamp_Backend.Services.Token;

public class ValidateTokenServices
{
    public static void ConfigureJwtOptions(JwtBearerOptions options, IConfiguration config)
    {
        options.Authority = config["TokenSettings:Issuer"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["TokenSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = config["TokenSettings:Audience"],
            ValidateLifetime = true
        };
    }

    public static bool TokenIsExpired(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwt);
        return token.ValidTo > DateTime.Now;
    }

    public static AuthToken ToToken(string jwt, string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwt);
        return new AuthToken
        {
            IdToken = jwt,
            IdentityId = token.Claims.First(c => c.Type == "user_id").Value,
            RefreshToken = refreshToken
        };
    }
}

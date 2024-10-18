using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services.Firebase;
using Timestamp_Backend.Services.MongoDB;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThemesController(MongoDBServices database, IAuthenticationServices authenticationService) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationService;

    [HttpPost("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedThemeRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateThemeRecord createTheme)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Theme theme = createTheme.ToTheme();
        await _database.Themes.InsertOneAsync(theme);
        return CreatedAtRoute(new {id = theme.Id}, ReturnAuthorizedThemeRecord.FromTheme(theme, newToken));
    }

    [HttpGet("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedThemesRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedThemesRecord.FromThemes(await _database.Themes.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedThemeRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedThemeRecord.FromTheme(await _database.Themes.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }

    [HttpPut("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateThemeRecord updateTheme)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Theme theme = await _database.Themes.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(theme is null) return NotFound();
        theme = updateTheme.Update(theme);
        await _database.Themes.ReplaceOneAsync(p => p.Id == id, theme);
        return Ok(newToken);
    }

    [HttpDelete("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Theme theme = await _database.Themes.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(theme is null) return NotFound();
        await _database.Themes.DeleteOneAsync(p => p.Id == id);
        return Ok(newToken);
    }

    [HttpGet("client")]
    public async Task<ActionResult<ReturnUnauthorizedThemesRecord>> GetForClient()
    {
        return Ok(ReturnUnauthorizedThemesRecord.FromThemes(await _database.Themes.Find(_ => true).ToListAsync()));
    }
    [HttpGet("client/{id}")]
    public async Task<ActionResult<ReturnUnauthorizedThemeRecord>> GetByIdForClient(string id)
    {
        return Ok(ReturnUnauthorizedThemeRecord.FromTheme(await _database.Themes.Find(p => p.Id == id).FirstOrDefaultAsync()));
    }
}

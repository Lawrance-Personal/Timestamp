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
public class FramesController(MongoDBServices database, IAuthenticationServices authenticationService) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationService;

    [HttpPost("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedFrameRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateFrameRecord createFrame)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Frame frame = createFrame.ToFrame();
        await _database.Frames.InsertOneAsync(frame);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Created Frame " + frame.Name,
        });
        return CreatedAtRoute(new {id = frame.Id}, ReturnAuthorizedFrameRecord.FromFrame(frame, newToken));
    }

    [HttpGet("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedFramesRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedFramesRecord.FromFrames(await _database.Frames.Find(_ => true).ToListAsync(), newToken));
    }
    
    [HttpGet("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedFrameRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedFrameRecord.FromFrame(await _database.Frames.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("admin/theme")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedFramesRecord>> GetByThemeId([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedFramesRecord.FromFrames(await _database.Frames.Find(p => p.ThemeId == id).ToListAsync(), newToken));
    }
    [HttpGet("admin/booth")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedFramesRecord>> GetByBoothId([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Booth booth = await _database.Booths.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(booth is null) return NotFound();
        return Ok(ReturnAuthorizedFramesRecord.FromFrames(await _database.Frames.Find(p => booth.FrameIds.Contains(p.Id)).ToListAsync(), newToken));
    }

    [HttpPut("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateFrameRecord updateFrame)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Frame frame = await _database.Frames.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(frame is null) return NotFound();
        frame = updateFrame.Update(frame);
        await _database.Frames.ReplaceOneAsync(p => p.Id == id, frame);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Updated Frame " + frame.Name,
        });
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
        Frame frame = await _database.Frames.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(frame is null) return NotFound();
        await _database.Frames.DeleteOneAsync(p => p.Id == id);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Deleted Frame " + frame.Name,
        });
        return Ok(newToken);
    }

    [HttpGet("client")]
    public async Task<ActionResult<ReturnUnauthorizedFramesRecord>> GetForClient()
    {
        return Ok(ReturnUnauthorizedFramesRecord.FromFrames(await _database.Frames.Find(_ => true).ToListAsync()));
    }
    [HttpGet("client/{id}")]
    public async Task<ActionResult<ReturnUnauthorizedFrameRecord>> GetByIdForClient(string id)
    {
        return Ok(ReturnUnauthorizedFrameRecord.FromFrame(await _database.Frames.Find(p => p.Id == id).FirstOrDefaultAsync()));
    }
    [HttpGet("client/theme")]
    public async Task<ActionResult<ReturnUnauthorizedFramesRecord>> GetByThemeIdForClient(string id)
    {
        return Ok(ReturnUnauthorizedFramesRecord.FromFrames(await _database.Frames.Find(p => p.ThemeId == id).ToListAsync()));
    }
    [HttpGet("client/booth")]
    public async Task<ActionResult<ReturnUnauthorizedFramesRecord>> GetByBoothIdForClient(string id)
    {
        Booth booth = await _database.Booths.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(booth is null) return NotFound();
        return Ok(ReturnUnauthorizedFramesRecord.FromFrames(await _database.Frames.Find(p => booth.FrameIds.Contains(p.Id)).ToListAsync()));
    }
}

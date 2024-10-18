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
        Frame frame = await _database.Frames.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(frame is null) return NotFound();
        return Ok(ReturnAuthorizedFrameRecord.FromFrame(frame, newToken));
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
}

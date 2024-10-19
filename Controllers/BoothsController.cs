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
public class BoothsController(MongoDBServices database, IAuthenticationServices authenticationServices) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationServices;

    [HttpPost("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedBoothRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateBoothRecord createBooth)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Booth booth = createBooth.ToBooth();
        await _database.Booths.InsertOneAsync(booth);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Created Booth " + booth.Title,
        });
        return CreatedAtRoute(new {id = booth.Id}, ReturnAuthorizedBoothRecord.FromBooth(booth, newToken));
    }

    [HttpGet("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedBoothsRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedBoothsRecord.FromBooths(await _database.Booths.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedBoothRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Booth booth = await _database.Booths.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(booth is null) return NotFound();
        return Ok(ReturnAuthorizedBoothRecord.FromBooth(booth, newToken));
    }

    [HttpPut("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateBoothRecord updateBooth)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Booth booth = await _database.Booths.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(booth is null) return NotFound();
        booth = updateBooth.Update(booth);
        await _database.Booths.ReplaceOneAsync(p => p.Id == id, booth);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Updated Booth " + booth.Title,
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
        Booth booth = await _database.Booths.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(booth is null) return NotFound();
        await _database.Booths.DeleteOneAsync(p => p.Id == id);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Deleted Booth " + booth.Title,
        });
        return Ok(newToken);
    }

    [HttpGet("client/{id}")]
    public async Task<ActionResult<ReturnUnauthorizedBoothRecord>> GetByIdForClient([FromRoute] string id)
    {
        return Ok(ReturnUnauthorizedBoothRecord.FromBooth(await _database.Booths.Find(p => p.Id == id).FirstOrDefaultAsync()));
    }
}

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
public class AdminsController(MongoDBServices database, IAuthenticationServices authenticationService) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationService;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedAdminRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, RegisterAdminRecord registerAdmin)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Admin admin = registerAdmin.ToCreateAdminRecord().ToAdmin();
        var identityId = await _authenticationService.Register(registerAdmin.Email, registerAdmin.Password);
        if(identityId == null) return BadRequest("Email Address Already Exist");
        admin.IdentityId = identityId;
        await _database.Admins.InsertOneAsync(admin);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Created New Admin " + admin.Name,
        });
        return CreatedAtRoute(new {id = admin.Id}, ReturnAuthorizedAdminRecord.FromAdmin(admin, newToken));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ReturnAuthorizedAdminRecord>> Login(LoginRecord credential)
    {
        AuthToken? token = await _authenticationService.Login(credential.Email, credential.Password);
        if(token.IdToken == null) return BadRequest("Invalid Credential");
        Admin a = await _database.Admins.Find(p => p.IdentityId == token.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Logged In",
        });
        return Ok(ReturnAuthorizedAdminRecord.FromAdmin(await _database.Admins.Find(p => p.IdentityId == token.IdentityId).FirstOrDefaultAsync(), token));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedAdminRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedAdminsRecord.FromAdmins(await _database.Admins.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedAdminRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedAdminRecord.FromAdmin(await _database.Admins.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateAdminRecord adminIn)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Admin admin = await _database.Admins.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(admin == null) return NotFound();
        admin = adminIn.Update(admin);
        await _database.Admins.ReplaceOneAsync(p => p.Id == id, admin);
        if(adminIn.Email != null) _authenticationService.UpdateEmail(admin.IdentityId, adminIn.Email);
        if(adminIn.Password != null) _authenticationService.UpdatePassword(admin.IdentityId, adminIn.Password);
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = admin.Id,
            Message = "Updated Admin " + admin.Name,
        });
        return Ok(newToken);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Admin admin = await _database.Admins.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(admin == null) return NotFound();
        _authenticationService.Unregister(admin.IdentityId);
        await _database.Admins.DeleteOneAsync(p => p.Id == id);
        Admin a = await _database.Admins.Find(p => p.IdentityId == newToken.IdentityId).FirstOrDefaultAsync();
        await _database.Logs.InsertOneAsync(new Log{
            AdminId = a.Id,
            Message = "Deleted Admin " + admin.Name,
        });
        return Ok(newToken);
    }
}

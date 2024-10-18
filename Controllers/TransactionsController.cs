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
public class TransactionsController(MongoDBServices database, IAuthenticationServices authenticationService) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationService;

    [HttpGet("admin")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedTransactionsRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedTransactionsRecord.FromTransactions(await _database.Transactions.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("admin/{id}")]
    [Authorize]
    public async Task<ActionResult<ReturnAuthorizedTransactionRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id)
    {
        var tokenArr = token.Split(" ");
        var newToken = ValidateTokenServices.ToToken(tokenArr[1], refreshToken);
        if(ValidateTokenServices.TokenIsExpired(tokenArr[1])){
            newToken = await _authenticationService.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnAuthorizedTransactionRecord.FromTransaction(await _database.Transactions.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }

    [HttpPost("client")]
    public async Task<ActionResult<ReturnUnauthorizedTransactionRecord>> Create(CreateTransactionRecord createTransaction)
    {
        Transaction transaction = createTransaction.ToTransaction();
        Page page = createTransaction.ToPage();
        await _database.Transactions.InsertOneAsync(transaction);
        await _database.Pages.InsertOneAsync(page);
        return CreatedAtRoute(new { id = transaction.Id }, ReturnUnauthorizedTransactionRecord.FromTransaction(transaction, page));
    }
}
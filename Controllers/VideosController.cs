using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services.Firebase;
using Timestamp_Backend.Services.MongoDB;

namespace Timestamp_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideosController(MongoDBServices database, IAuthenticationServices authenticationService) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationService;

    [HttpPost]
    public async Task<ActionResult<ReturnUnauthorizedVideoRecord>> Create(CreateVideoRecord createVideo)
    {
        Video video = createVideo.ToVideo();
        await _database.Videos.InsertOneAsync(video);
        return CreatedAtRoute(new { id = video.Id }, ReturnUnauthorizedVideoRecord.FromVideo(video));
    }

    [HttpGet("id")]
    public async Task<ActionResult<ReturnUnauthorizedVideoRecord>> GetById(string id)
    {
        return Ok(ReturnUnauthorizedVideoRecord.FromVideo(await _database.Videos.Find(p => p.Id == id).FirstOrDefaultAsync()));
    }
}

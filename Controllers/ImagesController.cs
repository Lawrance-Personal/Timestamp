using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services.Firebase;
using Timestamp_Backend.Services.MongoDB;

namespace Timestamp_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController(MongoDBServices database, IAuthenticationServices authenticationService) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authenticationService = authenticationService;

    [HttpPost]
    public async Task<ActionResult<ReturnUnauthorizedImageRecord>> Create(CreateImageRecord createImage)
    {
        Image image = createImage.ToImage();
        await _database.Images.InsertOneAsync(image);
        return CreatedAtRoute(new { id = image.Id }, ReturnUnauthorizedImageRecord.FromImage(image));
    }

    [HttpGet]
    public async Task<ActionResult<ReturnUnauthorizedImagesRecord>> Get()
    {
        return Ok(ReturnUnauthorizedImagesRecord.FromImages(await _database.Images.Find(p => true).ToListAsync()));
    }
    [HttpGet("id")]
    public async Task<ActionResult<ReturnUnauthorizedImageRecord>> GetById(string id)
    {
        return Ok(ReturnUnauthorizedImageRecord.FromImage(await _database.Images.Find(p => p.Id == id).FirstOrDefaultAsync()));
    }
}

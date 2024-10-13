using System;
using Microsoft.AspNetCore.Mvc;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services.Firebase;
using Timestamp_Backend.Services.MongoDB;

namespace Timestamp_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController(MongoDBServices database) : ControllerBase
{
    private readonly MongoDBServices _database = database;

    [HttpPost]
    public async Task<ActionResult<ReturnImageRecord>> Create(CreateImageRecord createImage)
    {
        Image image = createImage.ToImage();
        await _database.Images.InsertOneAsync(image);
        return CreatedAtRoute(new {id = image.Id}, ReturnImageRecord.FromImage(image));
    }
}

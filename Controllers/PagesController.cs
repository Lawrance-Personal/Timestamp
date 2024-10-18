using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services.MongoDB;

namespace Timestamp_Backend.Controllers;

[ApiController]
[Route("views/[controller]")]
public class PagesController(MongoDBServices database) : Controller
{
    private readonly MongoDBServices _database = database;

    [HttpGet("{id:length(24)}")]
    public async Task<ViewResult> Index(string id)
    {
        Page page = await _database.Pages.Find(p => p.Id == id).FirstOrDefaultAsync();
        if(page == null) return View("Views/Pages/invalid.cshtml");
        else{
            List<Image> images = await _database.Images.Find(p => page.ImageIds.Contains(p.Id)).ToListAsync();
            Video video = await _database.Videos.Find(p => p.Id == page.VideoId).FirstOrDefaultAsync();
            ViewBag.images = images;
            ViewBag.video = video;
            return View("Views/Pages/index.cshtml");
        }
    }
}

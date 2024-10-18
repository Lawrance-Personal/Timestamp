using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public class Booth
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("FrameIds")]
    public List<string> FrameIds { get; set; } = [];
}

public record ReturnAuthorizedBoothRecord
{
    [BsonElement("Booth")]
    public Booth Booth { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedBoothRecord FromBooth(Booth booth, AuthToken token)
    {
        return new ReturnAuthorizedBoothRecord
        {
            Booth = booth,
            Token = token
        };
    }
}

public record ReturnAuthorizedBoothsRecord
{   
    [BsonElement("Booths")]
    public List<Booth> Booths { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedBoothsRecord FromBooths(List<Booth> booths, AuthToken token)
    {
        return new ReturnAuthorizedBoothsRecord
        {
            Booths = booths,
            Token = token
        };
    }
}

public record ReturnUnauthorizedBoothRecord
{
    [BsonElement("Booth")]
    public Booth Booth { get; set; } = null!;

    public static ReturnUnauthorizedBoothRecord FromBooth(Booth booth)
    {
        return new ReturnUnauthorizedBoothRecord
        {
            Booth = booth
        };
    }
}

public record CreateBoothRecord
{
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("FrameIds")]
    public List<string> FrameIds { get; set; } = [];

    public Booth ToBooth()
    {
        return new Booth
        {
            Title = Title,
            Description = Description,
            ThemeId = ThemeId,
            FrameIds = FrameIds
        };
    }
}

public record UpdateBoothRecord
{
    [BsonElement("Title")]
    public string? Title { get; set; } = null!;
    [BsonElement("Description")]
    public string? Description { get; set; } = null!;
    [BsonElement("ThemeId")]
    public string? ThemeId { get; set; } = null!;
    [BsonElement("FrameIds")]
    public List<string>? FrameIds { get; set; } = null!;
    
    public Booth Update(Booth booth)
    {
        return new Booth
        {
            Id = booth.Id,
            Title = Title?? booth.Title,
            Description = Description?? booth.Description,
            ThemeId = ThemeId?? booth.ThemeId,
            FrameIds = FrameIds?? booth.FrameIds
        };
    }
}
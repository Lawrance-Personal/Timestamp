using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public record Layout{
    [JsonPropertyName("X")]
    public double X { get; set; }
    [JsonPropertyName("Y")]
    public double Y { get; set; }
    [JsonPropertyName("Width")]
    public double Width { get; set; }
    [JsonPropertyName("Height")]
    public double Height { get; set; }
}

public class Frame
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("PictureCount")]
    public int PictureCount { get; set; } = 0;
    [BsonElement("Layouts")]
    public List<Layout> Layouts { get; set; } = [];
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;
}

public record ReturnAuthorizedFrameRecord
{
    [BsonElement("Frame")]
    public Frame Frame { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedFrameRecord FromFrame(Frame frame, AuthToken token)
    {
        return new ReturnAuthorizedFrameRecord
        {
            Frame = frame,
            Token = token
        };
    }
}

public record ReturnAuthorizedFramesRecord
{
    [BsonElement("Frames")]
    public List<Frame> Frames { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnAuthorizedFramesRecord FromFrames(List<Frame> frames, AuthToken token)
    {
        return new ReturnAuthorizedFramesRecord
        {
            Frames = frames,
            Token = token
        };
    }
}

public record ReturnUnauthorizedFrameRecord
{
    [BsonElement("Frame")]
    public Frame Frame { get; set; } = null!;
    
    public static ReturnUnauthorizedFrameRecord FromFrame(Frame frame)
    {
        return new ReturnUnauthorizedFrameRecord
        {
            Frame = frame
        };
    }
}

public record ReturnUnauthorizedFramesRecord
{
    [BsonElement("Frames")]
    public List<Frame> Frames { get; set; } = [];
    
    public static ReturnUnauthorizedFramesRecord FromFrames(List<Frame> frames)
    {
        return new ReturnUnauthorizedFramesRecord
        {
            Frames = frames
        };
    }
}

public record CreateFrameRecord
{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("PictureCount")]
    public int PictureCount { get; set; } = 0;
    [BsonElement("Layouts")]
    public List<Layout> Layouts { get; set; } = [];
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;

    public Frame ToFrame()
    {
        return new Frame
        {
            Name = Name,
            PictureCount = PictureCount,
            Layouts = Layouts,
            ThemeId = ThemeId
        };
    }
}

public record UpdateFrameRecord
{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("PictureCount")]
    public int PictureCount { get; set; } = 0;
    [BsonElement("Layouts")]
    public List<Layout> Layouts { get; set; } = [];
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;

    public Frame Update(Frame frame)
    {
        return new Frame
        {
            Id = frame.Id,
            Name = Name?? frame.Name,
            PictureCount = PictureCount == 0? frame.PictureCount : PictureCount,
            Layouts = Layouts?? frame.Layouts,
            ThemeId = frame.ThemeId,
            Url = Url?? frame.Url
        };
    }
}

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class Video
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;
}

public record ReturnUnauthorizedVideoRecord
{
    [BsonElement("Video")]
    public Video Video { get; set; } = null!;

    public static ReturnUnauthorizedVideoRecord FromVideo(Video video)
    {
        return new ReturnUnauthorizedVideoRecord
        {
            Video = video,
        };
    }
}

public record CreateVideoRecord
{
    [BsonElement("Url")]
    public string Url { get; set; } = null!;

    public Video ToVideo()
    {
        return new Video
        {
            Url = Url
        };
    }
}
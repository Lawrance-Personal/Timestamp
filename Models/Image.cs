using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public class Image
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public record ReturnUnauthorizedImageRecord
{
    [BsonElement("Image")]
    public Image Image { get; set; } = null!;

    public static ReturnUnauthorizedImageRecord FromImage(Image image)
    {
        return new ReturnUnauthorizedImageRecord
        {
            Image = image,
        };
    }
}

public record ReturnUnauthorizedImagesRecord
{
    [BsonElement("Images")]
    public List<Image> Images { get; set; } = [];

    public static ReturnUnauthorizedImagesRecord FromImages(List<Image> images)
    {
        return new ReturnUnauthorizedImagesRecord
        {
            Images = images,
        };
    }
}

public record CreateImageRecord
{
    [BsonElement("Url")]
    public string Url { get; set; } = null!;

    public Image ToImage()
    {
        return new Image
        {
            Url = Url
        };
    }
}
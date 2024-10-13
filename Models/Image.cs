using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Firebase;

namespace Timestamp_Backend.Models;

public class Image
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;
}

public record ReturnImageRecord
{
    [BsonElement("Image")]
    public Image Image { get; set; } = null!;

    public static ReturnImageRecord FromImage(Image image)
    {
        return new ReturnImageRecord
        {
            Image = image,
        };
    }
}

public record ReturnImagesRecord
{
    [BsonElement("Images")]
    public List<Image> Images { get; set; } = [];

    public static ReturnImagesRecord FromImages(List<Image> images)
    {
        return new ReturnImagesRecord
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
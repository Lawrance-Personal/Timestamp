using System;
using System.Text.Json.Nodes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public class Theme
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Config")]
    public string Config { get; set; } = null!;
}

public record ReturnAuthorizedThemeRecord
{
    [BsonElement("Theme")]
    public Theme Theme { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedThemeRecord FromTheme(Theme theme, AuthToken token)
    {
        return new ReturnAuthorizedThemeRecord
        {
            Theme = theme,
            Token = token
        };
    }    
}

public record ReturnAuthorizedThemesRecord
{
    [BsonElement("Themes")]
    public List<Theme> Themes { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedThemesRecord FromThemes(List<Theme> theme, AuthToken token)
    {
        return new ReturnAuthorizedThemesRecord
        {
            Themes = theme,
            Token = token
        };
    }
}

public record ReturnUnauthorizedThemeRecord
{
    [BsonElement("Theme")]
    public Theme Theme { get; set; } = null!;

    public static ReturnUnauthorizedThemeRecord FromTheme(Theme theme)
    {
        return new ReturnUnauthorizedThemeRecord
        {
            Theme = theme
        };
    }
}

public record ReturnUnauthorizedThemesRecord
{
    [BsonElement("Themes")]
    public List<Theme> Themes { get; set; } = [];
    
    public static ReturnUnauthorizedThemesRecord FromThemes(List<Theme> themes)
    {
        return new ReturnUnauthorizedThemesRecord
        {
            Themes = themes
        };
    }
}

public record CreateThemeRecord
{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Config")]
    public string Config { get; set; } = null!;

    public Theme ToTheme()
    {
        return new Theme
        {
            Name = Name,
            Config = Config
        };
    }
}

public record UpdateThemeRecord
{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Config")]
    public string Config { get; set; } = null!;

    public Theme Update(Theme theme)
    {
        return new Theme
        {
            Id = theme.Id,
            Name = Name?? theme.Name,
            Config = Config?? theme.Config
        };
    }
}
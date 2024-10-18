using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public class Log
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Message")]
    public string Message { get; set; } = null!;
    [BsonElement("Timestamp")]
    public string Timestamp { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    [BsonElement("AdminId")]
    public string AdminId { get; set; } = null!;
}

public record ReturnAuthorizedLogRecord
{
    [BsonElement("Log")]
    public Log Log { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedLogRecord FromLog(Log log, AuthToken token)
    {
        return new ReturnAuthorizedLogRecord
        {
            Log = log,
            Token = token
        };
    }
}

public record ReturnAuthorizedLogsRecord
{
    [BsonElement("Logs")]
    public List<Log> Logs { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnAuthorizedLogsRecord FromLogs(List<Log> logs, AuthToken token)
    {
        return new ReturnAuthorizedLogsRecord
        {
            Logs = logs,
            Token = token
        };
    }
}
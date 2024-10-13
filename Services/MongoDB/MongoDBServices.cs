using System;
using MongoDB.Driver;
using Timestamp_Backend.Models;

namespace Timestamp_Backend.Services.MongoDB;

public class MongoDBServices
{
    private readonly IMongoDatabase _database;
    public MongoDBServices(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Image> Images => _database.GetCollection<Image>("Images");
    public IMongoCollection<Admin> Admins => _database.GetCollection<Admin>("Admins");
}

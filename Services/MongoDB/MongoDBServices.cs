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

    public IMongoCollection<Admin> Admins => _database.GetCollection<Admin>("Admins");
    public IMongoCollection<Booth> Booths => _database.GetCollection<Booth>("Booths");
    public IMongoCollection<Frame> Frames => _database.GetCollection<Frame>("Frames");
    public IMongoCollection<Image> Images => _database.GetCollection<Image>("Images");
    public IMongoCollection<Log> Logs => _database.GetCollection<Log>("Logs");
    public IMongoCollection<Page> Pages => _database.GetCollection<Page>("Pages");
    public IMongoCollection<Theme> Themes => _database.GetCollection<Theme>("Themes");
    public IMongoCollection<Transaction> Transactions => _database.GetCollection<Transaction>("Transactions");
    public IMongoCollection<Video> Videos => _database.GetCollection<Video>("Videos");
}

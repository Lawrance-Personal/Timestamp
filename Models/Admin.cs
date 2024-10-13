using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Firebase;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public class Admin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("IdentityId")]
    public string IdentityId { get; set; } = null!;
}

public record ReturnAuthorizedAdminRecord
{
    [BsonElement("Admin")]
    public Admin Admin { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedAdminRecord FromAdmin(Admin admin, AuthToken token)
    {
        return new ReturnAuthorizedAdminRecord
        {
            Admin = admin,
            Token = token
        };
    }
}

public record ReturnAuthorizedAdminsRecord
{
    [BsonElement("Admins")]
    public List<Admin> Admins { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnAuthorizedAdminsRecord FromAdmins(List<Admin> admins, AuthToken token)
    {
        return new ReturnAuthorizedAdminsRecord
        {
            Admins = admins,
            Token = token
        };
    }
}

public record RegisterAdminRecord
{
    [BsonElement("Email")]
    public string Email { get; set; } = null!;
    [BsonElement("Password")]
    public string Password { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    public CreateAdminRecord ToCreateAdminRecord()
    {
        return new CreateAdminRecord
        {
            Name = Name
        };
    }
}

public record CreateAdminRecord
{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    public Admin ToAdmin()
    {
        return new Admin
        {
            Name = Name
        };
    }
}

public record LoginRecord
{
    [BsonElement("Email")]
    public string Email { get; set; } = null!;
    [BsonElement("Password")]
    public string Password { get; set; } = null!;
}

public record UpdateAdminRecord
{
    [BsonElement("Name")]
    public string? Name { get; set; } = null!;
    [BsonElement("Email")]
    public string? Email { get; set; } = null!;
    [BsonElement("Password")]
    public string? Password { get; set; } = null!;

    public Admin Update(Admin admin)
    {
        return new Admin
        {
            Id = admin.Id,
            Name = Name?? admin.Name,
            IdentityId = admin.IdentityId
        };
    }
}
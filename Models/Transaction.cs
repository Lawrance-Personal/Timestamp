using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Timestamp_Backend.Services.Token;

namespace Timestamp_Backend.Models;

public class Transaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("BoothId")]
    public string BoothId { get; set; } = null!;
    [BsonElement("Amount")]
    public double Amount { get; set; } = 0;
    [BsonElement("PaymentType")]
    public string PaymentType { get; set; } = null!;
    [BsonElement("Timestamp")]
    public string Timestamp { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    [BsonElement("PageId")]
    public string PageId { get; set; } = null!;
}

public record ReturnAuthorizedTransactionRecord
{
    [BsonElement("Transaction")]
    public Transaction Transaction { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnAuthorizedTransactionRecord FromTransaction(Transaction transaction, AuthToken token)
    {
        return new ReturnAuthorizedTransactionRecord
        {
            Transaction = transaction,
            Token = token
        };
    }
}

public record ReturnAuthorizedTransactionsRecord
{
    [BsonElement("Transactions")]
    public List<ReturnAuthorizedTransactionRecord> Transactions { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnAuthorizedTransactionsRecord FromTransactions(List<Transaction> transactions, AuthToken token)
    {
        return new ReturnAuthorizedTransactionsRecord
        {
            Transactions = transactions.Select(t => ReturnAuthorizedTransactionRecord.FromTransaction(t, token)).ToList(),
            Token = token
        };
    }
}

public record ReturnUnauthorizedTransactionRecord
{
    [BsonElement("Transaction")]
    public Transaction Transaction { get; set; } = null!;
    
    public static ReturnUnauthorizedTransactionRecord FromTransaction(Transaction transaction)
    {
        return new ReturnUnauthorizedTransactionRecord
        {
            Transaction = transaction
        };
    }
}

public record CreateTransactionRecord
{
    [BsonElement("BoothId")]
    public string BoothId { get; set; } = null!;
    [BsonElement("Amount")]
    public double Amount { get; set; } = 0;
    [BsonElement("PaymentType")]
    public string PaymentType { get; set; } = null!;
    
    public Transaction ToTransaction()
    {
        return new Transaction
        {
            BoothId = BoothId,
            Amount = Amount,
            PaymentType = PaymentType,
        };
    }
}
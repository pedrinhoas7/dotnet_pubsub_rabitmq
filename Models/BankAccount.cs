// BankAccount.cs
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace signalr_backend.Models
{
    public class BankAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Document { get; set; }
        public string Owner { get; set; }
        public decimal Balance { get; set; }
    }
}


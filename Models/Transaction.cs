// Transaction.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace signalr_backend.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "deposito" ou "pagamento"
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dotnet_pubsub_rabitmq.Models
{
    public class BankAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string AccountNumber { get; set; }
        public string Branch { get; set; }
        public string HolderName { get; set; }
        public string Document { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

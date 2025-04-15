namespace dotnet_pubsub_rabitmq.Models
{
    public class Transaction
    {
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}

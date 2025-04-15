namespace dotnet_pubsub_rabitmq.Dtos
{
    public class TransactionDTO
    {
        public string AccountId { get; set; }
        public decimal Amount { get; set; } 
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}

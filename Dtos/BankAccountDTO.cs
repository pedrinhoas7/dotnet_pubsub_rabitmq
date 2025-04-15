namespace dotnet_pubsub_rabitmq.Dtos;

public class BankAccountDTO
{
    public string? Id { get; set; }

    public string AccountNumber { get; set; }
    public string Branch { get; set; }
    public string HolderName { get; set; }
    public string Document { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public DateTime LastUpdated { get; set; }

}

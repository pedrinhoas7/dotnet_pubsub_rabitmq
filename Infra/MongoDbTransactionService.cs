using signalr_backend.Models;
using MongoDB.Driver;

namespace signalr_backend.Infra;

public class MongoDbTransactionService
{
    private readonly IMongoCollection<Transaction> _transactions;

    public MongoDbTransactionService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDb:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDb:Database"]);
        _transactions = database.GetCollection<Transaction>("Transactions");
    }

    public async Task<List<Transaction>> GetAllAsync()
        => await _transactions.Find(_ => true).ToListAsync();

    public async Task<Transaction> GetByIdAsync(string id)
        => await _transactions.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Transaction transaction)
        => await _transactions.InsertOneAsync(transaction);

    public async Task DeleteAsync(string id)
        => await _transactions.DeleteOneAsync(x => x.Id == id);
}

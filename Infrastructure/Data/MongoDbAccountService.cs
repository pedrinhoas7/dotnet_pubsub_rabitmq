using MongoDB.Driver;
using signalr_backend.Domain.Entities;

namespace dotnet_pubsub_rabitmq.Infrastructure.Data;

public class MongoDbAccountService
{
    private readonly IMongoCollection<BankAccount> _bankAccounts;

    public MongoDbAccountService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDb:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDb:Database"]);
        _bankAccounts = database.GetCollection<BankAccount>("Accounts");
    }

    public async Task<List<BankAccount>> GetAllAsync()
        => await _bankAccounts.Find(_ => true).ToListAsync();

    public async Task<BankAccount> GetByIdAsync(string id)
        => await _bankAccounts.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<BankAccount> GetByDocument(string document)
    => await _bankAccounts.Find(x => x.Document == document).FirstOrDefaultAsync();

    public async Task CreateAsync(BankAccount account)
        => await _bankAccounts.InsertOneAsync(account);

    public async Task UpdateAsync(BankAccount account)
    {
        var bankAccount = await GetByDocument(account.Document);
        await _bankAccounts.ReplaceOneAsync(x => x.Id == bankAccount.Id, account);
    }

    public async Task DeleteAsync(string id)
        => await _bankAccounts.DeleteOneAsync(x => x.Id == id);
}

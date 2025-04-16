using dotnet_pubsub_rabitmq.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using signalr_backend.Domain.Entities;

namespace dotnet_pubsub_rabitmq.Web.Hubs;

public class TransactionWebSocket : Hub
{
    private static readonly Dictionary<string, string> ConnectedUsers = new();

    private readonly MongoDbAccountService _accountService;
    private readonly MongoDbTransactionService _transactionService;

    public TransactionWebSocket(MongoDbAccountService accountService, MongoDbTransactionService transactionService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext != null)
        {
            var email = httpContext.Request.Query["email"];
            var document = httpContext.Request.Query["document"];

            if (string.IsNullOrEmpty(email))
            {
                await Clients.Caller.SendAsync("Error", "Email não informado.");
                return;
            }

            if (string.IsNullOrEmpty(document))
            {
                await Clients.Caller.SendAsync("Error", "Documento não informado.");
                return;
            }

            if (Context.ConnectionId == null)
            {
                await Clients.Caller.SendAsync("Error", "ConnectionId não informado.");
                return;
            }

            Context.Items["email"] = email;
            Context.Items["document"] = document;
            ConnectedUsers[Context.ConnectionId] = document;

            var account = await _accountService.GetByDocument(document);
            if (account != null)
            {
                account.Owner = email;
                await _accountService.UpdateAsync(account);
            }
        }

        await base.OnConnectedAsync();
    }





    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var httpContext = Context.GetHttpContext();
        ConnectedUsers.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task GetAccountDetails()
    {
        if (!Context.Items.TryGetValue("email", out var email) || !Context.Items.TryGetValue("document", out var document))
        {
            await Clients.Caller.SendAsync("Error", "Email ou Documento não informado.");
            return;
        }

        var account = await _accountService.GetByDocument(document.ToString());
        if (account == null)
        {
            account = new BankAccount
            {
                Document = document.ToString(),
                Owner = email.ToString(),
                Balance = 0
            };
            await _accountService.CreateAsync(account);
        }

        var allTransactions = await _transactionService.GetAllAsync();
        var userTransactions = allTransactions
            .Where(e => e.AccountId == account.Id)
            .OrderByDescending(t => t.Timestamp)
            .Select(e => $"{e.Type.ToUpper()} - R$ {e.Amount} - {e.Description} - {e.Timestamp:dd/MM/yyyy HH:mm}")
            .ToList();

        await Clients.Caller.SendAsync("ReceiveAccountDetails", account.Balance, userTransactions);
    }



    public async Task ProcessTransaction(string type, decimal amount, string description)
    {
        if (!Context.Items.TryGetValue("email", out var email) || !Context.Items.TryGetValue("document", out var document))
        {
            await Clients.Caller.SendAsync("Error", "Email ou Documento não informado.");
            return;
        }

        var account = await _accountService.GetByDocument(document.ToString());
        if (account == null)
        {
            account = new BankAccount
            {
                Document = document.ToString(),
                Owner = email.ToString(),
                Balance = 0
            };
            await _accountService.CreateAsync(account);
        }

        if (type == "pagamento" && account.Balance < amount)
        {
            await Clients.Caller.SendAsync("Error", "Saldo insuficiente.");
            return;
        }

        account.Balance = type == "deposito"
            ? account.Balance + amount
            : account.Balance - amount;

        await _accountService.UpdateAsync(account);

        var transaction = new Transaction
        {
            AccountId = account.Id!,
            Amount = amount,
            Type = type,
            Description = description,
            Timestamp = DateTime.UtcNow,
            Status = "concluída"
        };

        await _transactionService.CreateAsync(transaction);

        var allTransactions = await _transactionService.GetAllAsync();
        var userTransactions = allTransactions
            .Where(e => e.AccountId == account.Id)
            .OrderByDescending(t => t.Timestamp)
            .Select(e => $"{e.Type.ToUpper()} - R$ {e.Amount} - {e.Description} - {e.Timestamp:dd/MM/yyyy HH:mm}")
            .ToList();

        await Clients.Caller.SendAsync("ReceiveAccountDetails", account.Balance, userTransactions);
    }

}

using Microsoft.AspNetCore.SignalR;
using dotnet_pubsub_rabitmq.Infra;
using dotnet_pubsub_rabitmq.Models;

namespace dotnet_pubsub_rabitmq.WebSocket;

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

            Console.WriteLine($"Documento recebido na conexão: {document}");
            Console.WriteLine($"Email recebido na conexão: {email}");

            // Verificar se o email foi informado
            if (string.IsNullOrEmpty(email))
            {
                await Clients.Caller.SendAsync("Error", "Email não informado.");
                return;
            }

            // Verificar se o document foi informado
            if (string.IsNullOrEmpty(document))
            {
                await Clients.Caller.SendAsync("Error", "Documento não informado.");
                return;
            }

            // Verificar se ConnectionId não é nulo
            if (Context.ConnectionId == null)
            {
                await Clients.Caller.SendAsync("Error", "ConnectionId não informado.");
                return;
            }

            // Armazenar o email no Context.Items
            Context.Items["email"] = email;
            Context.Items["document"] = document;

            // Adicionar o documento ao dicionário ConnectedUsers para controle
            ConnectedUsers[Context.ConnectionId] = document;

            // Atualizar a conta com o email
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
        // Recuperar o email e documento de Context.Items
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
                Owner = email.ToString(), // Usar o email, se presente
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
        // Recuperar o email e documento de Context.Items
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
                Owner =  email.ToString(), // Se o email estiver vazio, usa "Desconhecido"
                Balance = 0
            };
            await _accountService.CreateAsync(account);
        }

        if (type == "pagamento" && account.Balance < amount)
        {
            await Clients.Caller.SendAsync("Error", "Saldo insuficiente.");
            return;
        }

        // Atualiza saldo
        account.Balance = type == "deposito"
            ? account.Balance + amount
            : account.Balance - amount;

        await _accountService.UpdateAsync(account);

        // Cria transação
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

        // Envia dados atualizados ao cliente
        var allTransactions = await _transactionService.GetAllAsync();
        var userTransactions = allTransactions
            .Where(e => e.AccountId == account.Id)
            .OrderByDescending(t => t.Timestamp)
            .Select(e => $"{e.Type.ToUpper()} - R$ {e.Amount} - {e.Description} - {e.Timestamp:dd/MM/yyyy HH:mm}")
            .ToList();

        await Clients.Caller.SendAsync("ReceiveAccountDetails", account.Balance, userTransactions);
    }

}

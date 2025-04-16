using dotnet_pubsub_rabitmq.Queue;
using AutoMapper;
using dotnet_pubsub_rabitmq.Models;
using dotnet_pubsub_rabitmq.Infra;
using dotnet_pubsub_rabitmq.WebSocket;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddHostedService<RabbitMQListener>();
builder.Services.AddSingleton<MongoDbAccountService>();
builder.Services.AddSingleton<MongoDbTransactionService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); 
    });
});

builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors();

app.MapHub<TransactionWebSocket>("/ws/transactions");

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();

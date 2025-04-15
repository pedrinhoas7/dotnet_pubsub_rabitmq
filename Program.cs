using dotnet_pubsub_rabitmq.Queue;
using AutoMapper;
using dotnet_pubsub_rabitmq.Models;
using dotnet_pubsub_rabitmq.Infra;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddHostedService<RabbitMQListener>();
builder.Services.AddSingleton<MongoDbAccountService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

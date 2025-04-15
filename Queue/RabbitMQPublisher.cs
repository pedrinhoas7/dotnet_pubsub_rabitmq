using System.Text;
using System.Text.Json;
using dotnet_pubsub_rabitmq.Dtos;
using RabbitMQ.Client;

namespace dotnet_pubsub_rabitmq.Queue;

public class RabbitMQPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMQPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync(BankAccountDTO dto, CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:User"],
            Password = _configuration["RabbitMQ:Password"]
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        await using var channel = await connection.CreateChannelAsync();

        try
        {
            await channel.QueueDeclareAsync(
                queue: _configuration["RabbitMQ:Queue"],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );


            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dto));


            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _configuration["RabbitMQ:Queue"],
                mandatory: false,
                body: body,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao declarar fila: {ex.Message}");
        }


    }
}

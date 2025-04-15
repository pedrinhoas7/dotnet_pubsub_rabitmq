using AutoMapper;
using dotnet_pubsub_rabitmq.Models;
using dotnet_pubsub_rabitmq.Queue;
using dotnet_pubsub_rabitmq.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pubsub_rabitmq.Controllers
{
    [ApiController]
    [Route("Transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly RabbitMQPublisher _publisher;

        public TransactionsController(RabbitMQPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("payment/notify")]
        public async Task Create([FromBody] Transaction transaction)
        {
            await _publisher.PublishTransactionAsync(transaction);
        }

    }
}

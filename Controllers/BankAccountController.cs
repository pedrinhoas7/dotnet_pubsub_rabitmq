using AutoMapper;
using dotnet_pubsub_rabitmq.Infra;
using dotnet_pubsub_rabitmq.Models;
using dotnet_pubsub_rabitmq.Queue;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pubsub_rabitmq.Controllers
{
    [ApiController]
    [Route("notify")]
    public class BankAccountController : ControllerBase
    {
        private readonly RabbitMQPublisher _publisher;
        private readonly MongoDbAccountService _mongoService;

        public BankAccountController(
            RabbitMQPublisher publisher,
            MongoDbAccountService mongoDbService)
        {
            _publisher = publisher;
            _mongoService = mongoDbService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BankAccount account)
        {

            await _mongoService.CreateAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var account = await _mongoService.GetByIdAsync(id);
            if (account == null)
                return NotFound();

            return Ok(account);
        }
    }
}

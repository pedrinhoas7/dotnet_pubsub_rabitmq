using AutoMapper;
using dotnet_pubsub_rabitmq.Dtos;
using dotnet_pubsub_rabitmq.Models;
using dotnet_pubsub_rabitmq.Queue;
using dotnet_pubsub_rabitmq.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pubsub_rabitmq.Controllers
{
    [ApiController]
    [Route("notify")]
    public class BankAccountController : ControllerBase
    {
        private readonly RabbitMQPublisher _publisher;
        private readonly MongoDbAccountService _mongoService;
        private readonly IMapper _mapper;

        public BankAccountController(
            RabbitMQPublisher publisher,
            MongoDbAccountService mongoDbService,
            IMapper mapper)
        {
            _publisher = publisher;
            _mongoService = mongoDbService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BankAccountDTO dto)
        {
            var account = _mapper.Map<BankAccount>(dto);
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

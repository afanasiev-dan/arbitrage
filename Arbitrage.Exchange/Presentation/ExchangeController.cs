using Arbitrage.Domain;
using Arbitrage.Exchange.Application.Contracts;
using Arbitrage.Exchange.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Exchange.Presentation
{
    [Route("api/[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly ILogger<ExchangeController> _logger;
        private readonly IExchangeService _exchangeService;

        public ExchangeController(ILogger<ExchangeController> logger, IExchangeService exchangeService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        [HttpPost("exchanges")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddExchanges([FromBody] IEnumerable<string> exchangeModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponce {RetMsg = "Ошибка валидации, проверьте правильность входных данных"});

            List<ExchangeModel> exchanges = exchangeModel.Select(x => new ExchangeModel()
            {
                Name = x,
                Id = Guid.NewGuid()
            }).ToList();

            await _exchangeService.AddAsync(exchanges);
            return Ok(new ApiResponce());
        }

        [HttpGet("exchange")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExchange(
                [FromQuery] string exchange)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponce() { RetMsg = "Ошибка валидации, проверьте правильность входных данных" });

            var exchangeModel = await _exchangeService.GetByNameAsync(exchange);

            if (exchangeModel is null)
                return NotFound(new ApiResponce() { RetMsg = "Биржа не найдена" });

            return Ok(new ApiResponce { Result = exchangeModel.Name });
        }

        [HttpGet("exchanges")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExchanges()
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponce() { RetMsg = "Ошибка валидации, проверьте правильность входных данных" });

            var exchanges = await _exchangeService.GetAllAsync();

            if (exchanges is null || !exchanges.Any())
                return NotFound(new ApiResponce() { RetMsg = "Биржи не найдена" });

            List<string> exchangesNameResponce = exchanges.Select(x => x.Name).ToList();

            return Ok(new ApiResponce { Result = exchangesNameResponce});
        }
    }
}
using Arbitrage.Domain;
using Arbitrage.ExchangeDomain;
using Arbitrage.Symbols.Application.Contracts;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Symbols.Presentation
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyPairController : ControllerBase
    {
        private readonly ILogger<CurrencyPairController> _logger;
        private readonly ICurrencyPairService _currencyPairService;


        public CurrencyPairController(
            ILogger<CurrencyPairController> logger,
            ICurrencyPairService currencyPairService)
        {
            _logger = logger;
            _currencyPairService = currencyPairService;
        }

        [HttpGet("currency-pairs")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrencyPairs()
        {
            var result = await _currencyPairService.GetAllCurrencyPairsAsync();

            if (result == null || !result.Any())
                return NotFound(new ApiResponce() { RetMsg = "Пара не найдена" });

            var responce = result.Select(cp =>
            {
                var responce = new CurrencyPairResponceDto()
                {
                    Ticker = cp.Pair,
                    ExchangeName = cp.Exchange.Name,
                    BaseCoin = cp.BaseCoin.Name,
                    QuoteCoin = cp.QuoteCoin.Name,
                    MarketType = cp.MarketType
                };

                return responce;
            }).ToList();


            return Ok(new ApiResponce() { Result = responce });
        }

        [HttpPost("proxy")]
        public async Task<IActionResult> Proxy([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest(new ApiResponce() { RetMsg = "Не указан URL" });

            try
            {
                var response = await Network.PostAsync(url);
                return Content(response, "application/json");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка при проксировании запроса");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponce() { RetMsg = "Ошибка при проксировании запроса" });
            }
        }
    }
}
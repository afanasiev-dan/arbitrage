using Arbitrage.Domain;
using Arbitrage.Symbols.Application.Contracts;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Symbols.Presentation
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyPairContorller : ControllerBase
    {
        private readonly ILogger<CurrencyPairContorller> _logger;
        private readonly ICurrencyPairService _currencyPairService;


        public CurrencyPairContorller(
            ILogger<CurrencyPairContorller> logger,
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

            var uniquePairsByBaseTicker = result
                    .GroupBy(cp => cp.BaseCoin.Name)
                    .Select(g => g.First())
                    .ToList();

            var responce = uniquePairsByBaseTicker.Select(cp =>
            {
                var responce = new CurrencyPairResponceDto()
                {
                    ExchangeNameLong = cp.Exchange.Name,
                    SymbolNameLong = cp.BaseCoin.Name,
                    SymbolNameShort = cp.QuoteCoin.Name,
                    MarketType = cp.MarketType
                };

                return responce;
            }).ToList();


            return Ok(new ApiResponce() { Result = responce });
        }
    }
}
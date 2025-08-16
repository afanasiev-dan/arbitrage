using Arbitrage.Domain;
using Arbitrage.Symbols.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.Symbols
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoinController : ControllerBase
    {
        private readonly ICoinService _coinService;
        public CoinController(ICoinService symbolService)
        {
            _coinService = symbolService;
        }

        [HttpGet("coins")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSymbols()
        {
            var coins = await _coinService.GetAllAsync();
            
            if (coins == null || !coins.Any())
                return NotFound(new ApiResponce { RetMsg = "Монета не найдена"});
            
            return Ok(new ApiResponce { Result = coins.Select(x => x.Name).ToList()});
        }

        [HttpGet("coin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSymbols([FromQuery] IEnumerable<string> coinNames)  
        {
            var coins = await _coinService.GetByTickerAsync(coinNames);
            
            if (coins == null || !coins.Any())
                return NotFound(new ApiResponce { RetMsg = "Монета не найдена"});
            
            return Ok(new ApiResponce { Result = coins.Select(x => x.Name).ToList()});
        }

        [HttpPost("coin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSymbols([FromQuery] IEnumerable<string> coinNames)  
        {
            await _coinService.AddSymbolsAsync(coinNames);
            return Ok(new ApiResponce());
        }
    }
}
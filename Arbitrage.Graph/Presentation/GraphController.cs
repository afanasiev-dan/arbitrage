using Arbitrage.Domain;
using Arbitrage.Graph.Application;
using Arbitrage.Graph.Presentation.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.Graph.Presentation;

[ApiController]
[Route("api/[controller]")]
public class GraphController : ControllerBase
{
    private readonly CandleService _candlesService;
    public GraphController(CandleService candlesService)
    {
        _candlesService = candlesService;
    }

    [HttpGet("candles")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCandles([FromQuery] CandlesRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponce() { RetMsg = "Ошибка валидации, проверьте правильность входных данных" });

        dto.DateTo= DateTime.UtcNow;
        dto.DateFrom = DateTime.UtcNow.AddHours(-2);

        var candles = await _candlesService.GetCandles(dto.ExchangeName, dto.SymbolName, dto.DateFrom.Value, dto.DateTo.Value, dto.MarketType);

        if (candles is null || !candles.SpotCandleData.Any())
            return NotFound(new ApiResponce() { RetMsg = "Свечи не найдены" });

        return Ok(new ApiResponce() { Result = candles });
    }

    [HttpPost("spread-candles")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpreadCandles(
            [FromBody] SpreadCandlesRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponce() { RetMsg = "Ошибка валидации, проверьте правильность входных данных" });
        
        requestDto.DateTo= DateTime.Now;
        requestDto.DateFrom= DateTime.Now.AddHours(-2);

        var candles = await _candlesService.GetSpreadCandles(requestDto);

        if (candles is null || candles.SpreadCandleData is null || !candles.SpreadCandleData.Any())
            return NotFound(new ApiResponce() { RetMsg = "Свечи не найдены" });

        return Ok(new ApiResponce() { Result = candles });
    }
}
using Arbitrage.Domain;
using Arbitrage.Graph.Application.Services;
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

        dto.DateTo = DateTime.UtcNow;
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

        requestDto.DateTo = DateTime.Now;
        requestDto.DateFrom = DateTime.Now.AddHours(-2);

        var candles = await _candlesService.GetSpreadCandles(requestDto);

        if (candles is null || candles.SpreadCandleData is null || !candles.SpreadCandleData.Any())
            return NotFound(new ApiResponce() { RetMsg = "Свечи не найдены" });

        return Ok(new ApiResponce() { Result = candles });
    }

    [HttpPost("arbitrage-candles")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateArbitrageCandles([FromBody] IEnumerable<CreateArbitrageCandleDto> dtos)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponce() { RetMsg = "Ошибка валидации, проверьте правильность входных данных" });

        try
        {
            var result = await _candlesService.CreateRangeAsync(dtos);
            return CreatedAtAction(nameof(GetAllArbitrageCandles), new ApiResponce() { Result = result });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new ApiResponce() { RetMsg = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponce() { RetMsg = $"Ошибка при создании свечей: {ex.Message}" });
        }
    }

    [HttpDelete("arbitrage-candles")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteArbitrageCandles([FromBody] IEnumerable<Guid> ids)
    {
        try
        {
            await _candlesService.DeleteRangeAsync(ids);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponce() { RetMsg = $"Ошибка при удалении свечей: {ex.Message}" });
        }
    }

    [HttpGet("arbitrage-candles")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllArbitrageCandles()
    {
        try
        {
            var candles = await _candlesService.GetAllAsync();
            
            if (candles is null || !candles.Any())
                return NotFound(new ApiResponce() { RetMsg = "Арбитражные свечи не найдены" });

            return Ok(new ApiResponce() { Result = candles });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponce() { RetMsg = $"Ошибка при получении свечей: {ex.Message}" });
        }
    }

    [HttpPut("arbitrage-candles")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateArbitrageCandles([FromBody] IEnumerable<ArbitrageCandleDto> dtos)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponce() { RetMsg = "Ошибка валидации, проверьте правильность входных данных" });

        try
        {
            await _candlesService.UpdateRangeAsync(dtos);
            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new ApiResponce() { RetMsg = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponce() { RetMsg = $"Ошибка при обновлении свечей: {ex.Message}" });
        }
    }
}
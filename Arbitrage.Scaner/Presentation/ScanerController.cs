using Arbitrage.Domain;
using Arbitrage.Scaner.Application.Contracts;
using Arbitrage.Scaner.Presentation.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arbitrage.Scaner.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanerController(
        ILogger<ScanerController> logger,
        IScanerService scanerService) : ControllerBase
    {
        
        private readonly ILogger<ScanerController> _logger = logger;
        private readonly  IScanerService _scanerService = scanerService;

        [HttpGet("scaner")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScaners()
        {
            var result = await _scanerService.GetScaners();

            if (result == null || !result.Any())
                return NotFound(new ApiResponce() { RetMsg = "Данные не найдены" });

            var responce = result.Select(cp =>
            {
                var responce = new ScanerAddDataRequestDto()
                {
                    BaseCoinName = cp.BaseCoin.Name,
                    QuoteCoinName = cp.QuoteCoin.Name,
                    ExchangeNameLong = cp.ExchangeLong.Name,
                    ExchangeNameShort = cp.ExchangeShort.Name,
                    MarketTypeLong = cp.MarketTypeLong,
                    MarketTypeShort = cp.MarketTypeShort,
                    PurchasePriceLong = cp.PurchasePriceLong,
                    PurchasePriceShort = cp.PurchasePriceShort,
                    FundingRateLong = cp.FundingRateLong,
                    FundingRateShort = cp.FundingRateShort
                };

                return responce;
            }).ToList();

            return Ok(new ApiResponce() { Result = responce });
        }

        [HttpPost("scaner")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddScaners(IEnumerable<ScanerAddDataRequestDto> scanerModels)
        {
            var result = await _scanerService.AddScaners(scanerModels);

            if (!result)
                return BadRequest(new ApiResponce() { RetMsg = "Не удалось добавить данные" });

            return Ok(new ApiResponce() { RetMsg = "Данные успешно добавлены" });
        }
    }
}
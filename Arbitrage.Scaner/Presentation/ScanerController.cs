using Arbitrage.Domain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Scaner.Application.Contracts;
using Arbitrage.Scaner.Domain.Entities;
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
        private readonly IScanerService _scanerService = scanerService;

        [HttpPost("get_scaner")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScaners(FilterRequestDto filter)
        {
            var result = await _scanerService.GetScaners();

            if (result == null || !result.Any())
                return NotFound(new ApiResponce() { RetMsg = "Данные не найдены" });

            #region Filtered
            var filteredResult = new List<ScanerModel>();

            foreach (var s in result)
            {
                bool include = true;
                if (filter.SpotExchanges != null && s.MarketTypeLong == MarketType.Spot)
                {
                    if (!filter.SpotExchanges.Contains(s.ExchangeLong.Name))
                    {
                        include = false;
                    }
                }
                if (include && filter.FuturesExchanges != null)
                {
                    if (s.MarketTypeShort == MarketType.Futures && !filter.FuturesExchanges.Contains(s.ExchangeShort.Name))
                    {
                        include = false;
                    }

                    if (include && s.MarketTypeLong == MarketType.Futures && !filter.FuturesExchanges.Contains(s.ExchangeLong.Name))
                    {
                        include = false;
                    }
                }

                if (include)
                {
                    filteredResult.Add(s);
                }
            }

            if (!filteredResult.Any())
                return Ok(new ApiResponce() { RetMsg = "Данные не найдены по указанным фильтрам" });
            #endregion

            var responce = filteredResult.Select(cp =>
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
                    FundingRateShort = cp.FundingRateShort,
                    TickerLong = cp.TickerLong.Pair,
                    TickerShort = cp.TickerShort.Pair
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
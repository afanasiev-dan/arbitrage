using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Presentation.Dto;

namespace Arbitrage.Graph.Application.Contracts
{
    public interface ICandleService
    {
        Task<SpotCandlesResponceDto> GetCandles(string exchangeName, string symbolName, DateTime dateFrom, DateTime dateTo, MarketType marketType);
        Task<SpreadCandlesResponceDto> GetSpreadCandles(SpreadCandlesRequestDto requestDto);
    }
}
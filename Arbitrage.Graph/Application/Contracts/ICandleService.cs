using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Presentation.Dto;

namespace Arbitrage.Graph.Application.Contracts
{
    public interface ICandleService
    {
        Task<SpotCandlesResponceDto> GetCandles(string exchangeName, string symbolName, DateTime dateFrom, DateTime dateTo, MarketType marketType);
        Task<SpreadCandlesResponceDto> GetSpreadCandles(SpreadCandlesRequestDto requestDto);
        Task<IEnumerable<ArbitrageCandleDto>> CreateRangeAsync(IEnumerable<CreateArbitrageCandleDto> dtos);
        Task DeleteRangeAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<ArbitrageCandleDto>> GetAllAsync();
        Task UpdateRangeAsync(IEnumerable<ArbitrageCandleDto> dtos);
    }
}
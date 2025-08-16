using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Symbols.Domain.Contracts
{
    public interface IExchangeFormatRepository
    {
        Task<ExchangeFormat> GetByExchangeName(string exchangeName);
        Task AddAsync(ExchangeFormat exchangeFormat);
    }
}
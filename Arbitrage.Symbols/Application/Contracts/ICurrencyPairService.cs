using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;

namespace Arbitrage.Symbols.Application.Contracts
{
    public interface ICurrencyPairService
    {
        Task<IEnumerable<CurrencyPair>> GetAllCurrencyPairsAsync();    
        Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(IEnumerable<string> currencyPairs);    
        Task AddCurrencyPairsAsync(IEnumerable<CurrencyPairRequestDto>currencyPairs);
        Task UpdateCurrencyPairsAsync(IEnumerable<CurrencyPairRequestDto> currencyPairs);
    }
}
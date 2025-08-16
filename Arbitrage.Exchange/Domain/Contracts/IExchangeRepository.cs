using Arbitrage.Exchange.Domain.Entities;

namespace Arbitrage.Exchange.Domain.Contracts
{
    public interface IExchangeRepository
    {
        Task AddAsync(IEnumerable<ExchangeModel> exchanges);
        Task<ExchangeModel?> GetByNameAsync(string name); 
        Task<List<ExchangeModel>> GetAllAsync();
    }
}
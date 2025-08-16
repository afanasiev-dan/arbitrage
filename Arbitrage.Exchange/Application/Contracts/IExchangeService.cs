using Arbitrage.Exchange.Domain.Entities;

namespace Arbitrage.Exchange.Application.Contracts
{
    public interface IExchangeService
    {
        Task AddAsync(IEnumerable<ExchangeModel> exchanges);
        Task<ExchangeModel?> GetByNameAsync(string name); 
        Task<List<ExchangeModel>> GetAllAsync();
    }
}
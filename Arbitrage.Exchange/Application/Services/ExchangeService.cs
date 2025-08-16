using Arbitrage.Exchange.Application.Contracts;
using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.Exchange.Domain.Entities;

namespace Arbitrage.Exchange.Application.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository _exchangeRepository;
        public ExchangeService(IExchangeRepository exchangeRepository)
        {
            _exchangeRepository = exchangeRepository;
        }

        public async Task AddAsync(IEnumerable<ExchangeModel> exchanges)
        {
            if (exchanges == null || !exchanges.Any())
                throw new ArgumentException("Список бирж пуст");

            var exchangesDb = await _exchangeRepository.GetAllAsync();
            var existingNames = exchangesDb.Select(e => e.Name).ToHashSet();
            var exchangesNew = exchanges.Where(x => !existingNames.Contains(x.Name)).ToList();

            if (!exchangesNew.Any())
                throw new ArgumentException("Все биржи уже существуют в базе данных");

            await _exchangeRepository.AddAsync(exchangesNew);
        }

        public async Task<List<ExchangeModel>> GetAllAsync()
        {
            return await _exchangeRepository.GetAllAsync();
        }

        public async Task<ExchangeModel?> GetByNameAsync(string name)
        {
            return await _exchangeRepository.GetByNameAsync(name);
        }
    }
}
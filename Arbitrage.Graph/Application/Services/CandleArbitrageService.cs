using Arbitrage.Graph.Domain.Entities;
using Arbitrage.Graph.Presentation.Dto;

namespace Arbitrage.Graph.Application.Services
{
    public partial class CandleService
    {
       public async Task<IEnumerable<ArbitrageCandleDto>> CreateRangeAsync(IEnumerable<CreateArbitrageCandleDto> dtos)
        {
            var candles = new List<ArbitrageCandle>();

            foreach (var dto in dtos)
            {
                var candle = await CreateCandleFromDto(dto);
                candles.Add(candle);
            }

            var createdCandles = await  _repository.AddRangeAsync(candles);
            return createdCandles.Select(MapToDto);
        }

        public async Task DeleteRangeAsync(IEnumerable<Guid> ids)
        {
            await  _repository.DeleteRangeAsync(ids);
        }

        public async Task<IEnumerable<ArbitrageCandleDto>> GetAllAsync()
        {
            var candles = await  _repository.GetAllAsync();
            return candles.Select(MapToDto);
        }

        public async Task UpdateRangeAsync(IEnumerable<ArbitrageCandleDto> dtos)
        {
            var candles = new List<ArbitrageCandle>();
            
            foreach (var dto in dtos)
            {
                var candle = await UpdateCandleFromDto(dto);
                candles.Add(candle);
            }

            await  _repository.UpdateRangeAsync(candles);
        }

        private async Task<ArbitrageCandle> CreateCandleFromDto(CreateArbitrageCandleDto dto)
        {
            var exchangeLongId = await GetExchangeIdByName(dto.ExchangeLongName);
            var exchangeShortId = await GetExchangeIdByName(dto.ExchangeShortName);

            var baseCoinId = await GetCoinIdByName(dto.BaseCoinName);
            var quoteCoinId = await GetCoinIdByName(dto.QuoteCoinName);

            return new ArbitrageCandle
            {
                OpenTime = dto.OpenTime,
                Interval = dto.Interval,
                Open = dto.Open,
                High = dto.High,
                Low = dto.Low,
                Close = dto.Close,
                BaseCoinId = baseCoinId,
                QuoteCoinId = quoteCoinId,
                ExchangeLongId = exchangeLongId,
                MarketTypeLong = dto.MarketTypeLong,
                ExchangeShortId = exchangeShortId,
                MarketTypeShort = dto.MarketTypeShort
            };
        }

        private async Task<ArbitrageCandle> UpdateCandleFromDto(ArbitrageCandleDto dto)
        {
            var exchangeLongId = await GetExchangeIdByName(dto.ExchangeLongName);
            var exchangeShortId = await GetExchangeIdByName(dto.ExchangeShortName);

            var baseCoinId = await GetCoinIdByName(dto.BaseCoinName);
            var quoteCoinId = await GetCoinIdByName(dto.QuoteCoinName);

            return new ArbitrageCandle
            {
                Id = dto.Id,
                OpenTime = dto.OpenTime,
                Interval = dto.Interval,
                Open = dto.Open,
                High = dto.High,
                Low = dto.Low,
                Close = dto.Close,
                BaseCoinId = baseCoinId,
                QuoteCoinId = quoteCoinId,
                ExchangeLongId = exchangeLongId,
                MarketTypeLong = dto.MarketTypeLong,
                ExchangeShortId = exchangeShortId,
                MarketTypeShort = dto.MarketTypeShort
            };
        }

        private async Task<Guid> GetCoinIdByName(string coinName)
        {
            var coins = await _coinsRepository.GetAllAsync();

            if (coins is null || !coins.Any())
                throw new ArgumentNullException("Не найден символ");

            var coin = coins.FirstOrDefault(s => s.Name == coinName);
            if (coin is null)
                throw new ArgumentNullException("В базе данных не найдены биржи или символы с указанными id");
            
            return coin.Id;
        }

        private async Task<Guid> GetExchangeIdByName(string exchangeName)
        {
            var exchanges = await _exchangeRepository.GetAllAsync();

            if (exchanges is null || !exchanges.Any())
                throw new ArgumentNullException("Не найдена биржа");

            var exchange = exchanges.FirstOrDefault(e => e.Name == exchangeName);
            if (exchange is null)
                throw new ArgumentNullException($"В базе данных не найдена биржа с именем: {exchangeName}");


            return exchange.Id;
        }

        private ArbitrageCandleDto MapToDto(ArbitrageCandle candle)
        {
            return new ArbitrageCandleDto
            {
                Id = candle.Id,
                OpenTime = candle.OpenTime,
                Interval = candle.Interval,
                Open = candle.Open,
                High = candle.High,
                Low = candle.Low,
                Close = candle.Close,
                BaseCoinName = candle.BaseCoin?.Name ?? string.Empty,
                QuoteCoinName = candle.QuoteCoin?.Name ?? string.Empty,
                ExchangeLongName = candle.ExchangeLong?.Name ?? string.Empty,
                MarketTypeLong = candle.MarketTypeLong,
                ExchangeShortName = candle.ExchangeShort?.Name ?? string.Empty,
                MarketTypeShort = candle.MarketTypeShort
            };
        } 
    }
}
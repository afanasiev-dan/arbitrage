using Arbitrage.Exchange.Domain.Contracts;
using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Application.Contracts;
using Arbitrage.Graph.Domain;
using Arbitrage.Graph.Infastructure.ExchangeApiClients;
using Arbitrage.Graph.Presentation.Dto;
using Arbitrage.Symbols.Domain.Contracts;

namespace Arbitrage.Graph.Application.Services;

public partial class CandleService : ICandleService
{
    private readonly IExchangeApiClientFactory _strategyFactory;
    private readonly ICandleRepository _repository;
    private readonly IExchangeRepository _exchangeRepository;
    private readonly ICoinRepository _coinsRepository;
    private int MaxCandlesPerRequest = 100;

    public CandleService(
        IExchangeApiClientFactory strategyFactory,
        ICandleRepository repository,
        IExchangeRepository exchangeRepository,
        ICoinRepository symbolRepository)
    {
        _strategyFactory = strategyFactory;
        _repository = repository;
        _exchangeRepository = exchangeRepository;
        _coinsRepository = symbolRepository;
    }

    public async Task<SpotCandlesResponceDto> GetCandles(string exchangeName, string symbolName, DateTime dateFrom, DateTime dateTo, MarketType marketType)
    {
        var candles = await GetCandlesFromDbOrApi(exchangeName, symbolName, dateFrom, dateTo, marketType) ?? [];

        SpotCandlesResponceDto result = new() { SpotCandleData = []};

        if (!candles.Any()) throw new ArgumentNullException("Не найдены свечи");
        
        var candle = candles.FirstOrDefault();

        if (candle is null) throw new ArgumentNullException("Не найдены свечи");
        if (candle.ExchangeId == Guid.Empty) throw new ArgumentNullException("Не найдена биржa");
        if (candle.CurrencyPairId == Guid.Empty) throw new ArgumentNullException("Не найдена монета");


        result.ExchangeName = candle.Exchange!.Name;
        result.Symbol = candle.Pair!.Pair;
        result.Interval = candle.Interval;

        for (int i = 0; i < candles.Count; i++)
        {
            CandleDataDto spreadCandleData = new();
            spreadCandleData.OpenTime = candles[i].OpenTime;
            spreadCandleData.Open = candles[i].Open;
            spreadCandleData.Close = candles[i].Close;
            spreadCandleData.Low = candles[i].Low;
            spreadCandleData.High = candles[i].High;
            spreadCandleData.Volume = candles[i].Volume;
            result.SpotCandleData.Add(spreadCandleData);
        }

        return result;
    }

    public async Task<SpreadCandlesResponceDto> GetSpreadCandles(SpreadCandlesRequestDto requestDto)
    {
        var candlesLongTask = await GetCandlesFromDbOrApi(requestDto.ExchangeNameLong, requestDto.SymbolNameLong, requestDto.DateFrom, requestDto.DateTo, requestDto.MarketTypeLong);
        var candlesShortTask = await GetCandlesFromDbOrApi(requestDto.ExchangeNameShort, requestDto.SymbolNameShort, requestDto.DateFrom, requestDto.DateTo, requestDto.MarketTypeShort);

        //await Task.WhenAll(candlesLongTask, candlesShortTask);

        var candlesLong = candlesLongTask;//.result
        var candlesShort = candlesShortTask;//.result

        if (candlesLong is null || candlesShort is null || !candlesLong.Any() || !candlesShort.Any())
            throw new ArgumentNullException("Не найдены свечи");

        if (candlesLong.Count != candlesShort.Count)
            throw new Exception("Количество свечей на биржах отличается");

        SpreadCandlesResponceDto result = CandlesResponceDataCollect(requestDto, candlesLong, candlesShort);

        return result;
    }

    private SpreadCandlesResponceDto CandlesResponceDataCollect(SpreadCandlesRequestDto requestDto, List<Candle> candlesLong, List<Candle> candlesShort)
    {
        SpreadCandlesResponceDto result = new() { SpreadCandleData = [] };
        result.ExchangeNameLong = requestDto.ExchangeNameLong;
        result.ExchangeNameShort = requestDto.ExchangeNameShort;
        result.SymbolNameLong = requestDto.SymbolNameLong;
        result.SymbolNameShort = requestDto.SymbolNameShort;
        result.Interval = candlesLong.First().Interval;

        for (int i = 0; i < candlesLong.Count; i++)
        {
            CandleDataDto spreadCandleData = new();
            spreadCandleData.OpenTime = candlesLong[i].OpenTime;
            spreadCandleData.Open = GetSpreadOperation(candlesLong[i].Open, candlesShort[i].Open);
            spreadCandleData.Close = GetSpreadOperation(candlesLong[i].Close, candlesShort[i].Close);
            spreadCandleData.Low = GetSpreadOperation(candlesLong[i].Low, candlesShort[i].Low);
            spreadCandleData.High = GetSpreadOperation(candlesLong[i].High, candlesShort[i].High);
            spreadCandleData.Volume = candlesLong[i].Volume;
            result.SpreadCandleData.Add(spreadCandleData);
        }

        return result;
    }

    private decimal GetSpreadOperation(decimal candleLong, decimal candleShort)
    {
        return (candleShort / candleLong - 1)*100;
    }

    private async Task<List<Candle>> GetCandlesFromDbOrApi(string exchangeName, string symbolName, DateTime dateFrom, DateTime dateTo, MarketType marketType)
    {
        var exchanges = await _exchangeRepository.GetAllAsync();
        var coins = await _coinsRepository.GetAllAsync();

        if (exchanges is null || !exchanges.Any())
            throw new ArgumentNullException("Не найдена биржa");

        if (coins is null || !coins.Any())
            throw new ArgumentNullException("Не найден символ");

        var symbol = coins.FirstOrDefault(s => s.Name == symbolName);
        var exchange = exchanges.FirstOrDefault(e => e.Name == exchangeName);

        if (symbol is null || exchange is null)
            throw new ArgumentNullException("В базе данных не найдены биржи или символы с указанными id");
        
        // Получаем данные из базы
        var dbCandles = await _repository.GetCandlesAsync(exchange.Name, symbol.Name, dateFrom, dateTo);

        // Определяем периоды, для которых данных нет или они неполные
        var missingPeriods = FindMissingPeriods(dbCandles, dateFrom, dateTo);

        // Если все данные есть в базе и они актуальные
        if (!missingPeriods.Any())
        {
            var candleInterval = dbCandles.FirstOrDefault()?.Interval;
            var expectedCount = (int)((dateTo - dateFrom).TotalMinutes / candleInterval);

            return TrimCandlesToRequestedCount(dbCandles, dateFrom, dateTo, expectedCount);
        }

        // Загружаем недостающие данные с биржи
        var fetchedCandles = await FetchMissingCandles(symbol.Name, exchange.Name, missingPeriods, marketType);

        // Берём только уникальные свечки
        var newCandles = fetchedCandles
            .Where(c => !dbCandles
                .Select(c => c.OpenTime)
                .Contains(c.OpenTime))
            .GroupBy(c => c.OpenTime)
            .Select(grp => grp.First())
            .ToList();

        if (newCandles.Any())
            await _repository.BulkInsertAsync(newCandles);

        // Объединяем данные из базы и с биржи
        var result = MergeCandles(dbCandles, newCandles, dateFrom, dateTo);


        // Рассчитываем сколько свечей должно быть за запрошенный период
        var finalCandleInterval = result.FirstOrDefault()?.Interval;
        var finalExpectedCount = (int)((dateTo - dateFrom).TotalMinutes / finalCandleInterval);


        return TrimCandlesToRequestedCount(result, dateFrom, dateTo, finalExpectedCount);;
    }

    private List<Candle> TrimCandlesToRequestedCount(List<Candle> candles, DateTime dateFrom, DateTime dateTo, int requestedCount)
    {
        if (candles == null || !candles.Any())
            return candles;

        // Сортируем свечи по времени (от старых к новым)
        var sortedCandles = candles.OrderBy(c => c.OpenTime).ToList();

        // Если запрошенное количество больше или равно количеству свечей - возвращаем как есть
        if (requestedCount <= 0 || sortedCandles.Count <= requestedCount)
            return sortedCandles;

        // Берем последние N свечей (самые новые)
        return sortedCandles.Skip(sortedCandles.Count - requestedCount).ToList();
    }

    private async Task<List<Candle>> FetchMissingCandles(
        string symbol,
        string exchangeName,
        List<(DateTime Start, DateTime End)> missingPeriods,
        MarketType marketType)
    {
        IExchangeApiClient _exchangeClient = _strategyFactory.GetClient(exchangeName);

        if (marketType == MarketType.Spot)
            MaxCandlesPerRequest = _exchangeClient.MaxCandlesSpotPerRequest;
        else
            MaxCandlesPerRequest = _exchangeClient.MaxCandlesFuturePerRequest;

        var allCandles = new List<Candle>();

        foreach (var period in missingPeriods)
        {
            var (requestCount, interval) = CalculateRequests(period.Start, period.End);

            for (int i = 0; i < requestCount; i++)
            {
                DateTime batchStart = period.Start.AddTicks(interval.Ticks * i);
                DateTime batchEnd = (i == requestCount - 1)
                    ? period.End
                    : batchStart.AddTicks(interval.Ticks);

                if (batchEnd > period.End) batchEnd = period.End;

                List<Candle> candles = [];

                if (marketType == MarketType.Futures)
                    candles = await _exchangeClient.GetFutureCandlesAsync(
                        symbol,
                        "",
                        batchStart,
                        batchEnd);
                else
                    candles = await _exchangeClient.GetSpotCandlesAsync(
                        symbol,
                        "",
                        batchStart,
                        batchEnd);

                allCandles.AddRange(candles);

                // Задержка для соблюдения лимитов API
                await Task.Delay(100);
            }
        }

        return allCandles;
    }

    private List<Candle> MergeCandles(
        List<Candle> dbCandles, 
        List<Candle> fetchedCandles, 
        DateTime dateFrom, 
        DateTime dateTo)
    {
        var merged = new List<Candle>();
        merged.AddRange(dbCandles);
        merged.AddRange(fetchedCandles);
 
        // Удаляем дубликаты (если вдруг они есть)
        var distinctCandles = merged
            .GroupBy(c => c.OpenTime)
            .Select(g => g.First())
            .OrderByDescending(c => c.OpenTime)
            .ToList();

        string exchangeName = distinctCandles.FirstOrDefault() is not null ?
            distinctCandles.First().Exchange is not null ?
                distinctCandles.First().Exchange!.Name :
                "Unknown" : "Unknown";

        if (exchangeName == Exchanges.Htx)
            distinctCandles
                .Where(c => c.OpenTime.ToLocalTime() >= dateFrom && c.OpenTime.ToLocalTime() <= dateTo);
        else
            distinctCandles
                .Where(c => c.OpenTime >= dateFrom && c.OpenTime <= dateTo);

        return distinctCandles;
}

    private List<(DateTime Start, DateTime End)> FindMissingPeriods(
        List<Candle> existingCandles,
        DateTime dateFrom,
        DateTime dateTo)
    {
        var missingPeriods = new List<(DateTime, DateTime)>();

        // Если в базе вообще нет данных
        if (!existingCandles.Any())
        {
            missingPeriods.Add((dateFrom, dateTo));
            return missingPeriods;
        }

        // Проверяем начало периода
        var firstCandleTime = existingCandles.Min(c => c.OpenTime);
        if (firstCandleTime > dateFrom)
        {
            missingPeriods.Add((dateFrom, firstCandleTime));
        }

        // Проверяем конец периода
        var lastCandleTime = existingCandles.Max(c => c.OpenTime);
        if (lastCandleTime < dateTo)
        {
            missingPeriods.Add((lastCandleTime, dateTo));
        }

        // Проверяем промежутки между свечами
        var orderedCandles = existingCandles.OrderBy(c => c.OpenTime).ToList();
        for (int i = 0; i < orderedCandles.Count - 1; i++)
        {
            var current = orderedCandles[i];
            var next = orderedCandles[i + 1];

            // Если между свечами больше 5 минут (или другого интервала)
            if (next.OpenTime - current.OpenTime > TimeSpan.FromMinutes(5))
                missingPeriods.Add((current.OpenTime, next.OpenTime));
        }

        return missingPeriods;
    }

    private (int requestCount, TimeSpan interval) CalculateRequests(
        DateTime dateFrom,
        DateTime dateTo)
    {
        // TimeSpan totalSpan = dateTo - dateFrom;
        // var candlesCount = totalSpan.TotalDays * CandlesPeriod.FiveMinPeriodCount;

        // // Если период меньше 1000 свечей - один запрос
        // if (candlesCount  <= MaxCandlesPerRequest)
        //     return (1, totalSpan);

        // // Рассчитываем интервал для пакетных запросов
        // double daysPerRequest = candlesCount / Math.Ceiling(totalSpan.TotalDays / MaxCandlesPerRequest);
        // int requestCount = (int)Math.Ceiling(candlesCount / CandlesPeriod.FiveMinPeriodCount);
        // TimeSpan interval = TimeSpan.FromDays(daysPerRequest);
        // return (requestCount, interval);

        if (dateTo <= dateFrom) return (0, TimeSpan.Zero);

        TimeSpan totalSpan = dateTo - dateFrom;
        double totalCandles = totalSpan.TotalDays * CandlesPeriod.FiveMinPeriodCount;

        // Если общее количество свечей меньше или равно максимальному - один запрос
        if (totalCandles <= MaxCandlesPerRequest)
            return (1, totalSpan);

        // Рассчитываем необходимое количество запросов
        int requestCount = (int)Math.Ceiling(totalCandles / MaxCandlesPerRequest);

        // Рассчитываем интервал для каждого запроса (в днях)
        double totalDays = totalSpan.TotalDays;
        double daysPerRequest = totalSpan.TotalDays / requestCount;
        TimeSpan interval = TimeSpan.FromDays(daysPerRequest);

        return (requestCount, interval);
    }
    
    private async Task<(List<Candle> newCandles, List<Candle> updatedCandles)> CompareAndUpdateCandles(
        List<Candle> dbCandles, 
        List<Candle> exchangeCandles)
    {
        var newCandles = new List<Candle>();
        var updatedCandles = new List<Candle>();
    
        // Группируем свечи по времени для быстрого поиска
        var dbCandlesDict = dbCandles.ToDictionary(c => c.OpenTime);
        var exchangeCandlesDict = exchangeCandles.ToDictionary(c => c.OpenTime);

        // 1. Находим новые свечи (которых нет в БД)
        newCandles = exchangeCandles
            .Where(ec => !dbCandlesDict.ContainsKey(ec.OpenTime))
            .ToList();

        // 2. Находим свечи, которые отличаются (и нуждаются в обновлении)
        foreach (var ec in exchangeCandles)
        {
            if (dbCandlesDict.TryGetValue(ec.OpenTime, out var dbCandle))
            {
                // Сравниваем все значимые поля свечи
                if (!CandleEquals(dbCandle, ec))
                {
                    updatedCandles.Add(ec);
                }
            }
        }

        return (newCandles, updatedCandles);
    }

    private bool CandleEquals(Candle a, Candle b)
    {
        return a.Open == b.Open &&
               a.High == b.High &&
               a.Low == b.Low &&
               a.Close == b.Close &&
               a.Volume == b.Volume &&
               a.Exchange == b.Exchange &&
               a.Pair == b.Pair;
        // Можно добавить другие поля, если они есть
    }
}
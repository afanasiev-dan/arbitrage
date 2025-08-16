using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;
using Arbitrage.Service.Base;
using Arbitrage.Service.Gateio;
using Arbitrage.Service.HTX;
using Arbitrage.Service.LBank;
using Arbitrage.Service.Mexc;
using Newtonsoft.Json;

namespace Arbitrage.Core.App
{
    public class ExchangeManager
    {
        public List<Exchange> Exchanges = new();
        public List<Crypto> Cryptos = new();

        public async Task Start()
        {
            // Этап 1: Создаем биржи, загружаем информацию о монетах
            MyTimer timer = new();
            Console.WriteLine($"Загружаем информацию..");
            var exchangeLoadTasks = LaunchConfig.ExchangeAssets.Select(async exchangeAsset =>
            {
                try
                {
                    var exchange = CreateExchange(exchangeAsset);
                    Exchanges.Add(exchange);
                    var infoCryptos = await exchange.GetCoins(exchangeAsset.AssetType) ?? [];
                    return (Exchange: exchange, InfoCryptos: infoCryptos, AssetType: exchangeAsset.AssetType.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{exchangeAsset.Print()}] Ошибка загрузки: {ex.Message}");
                    return (Exchange: null, InfoCryptos: null, AssetType: string.Empty);
                }
            }).ToList();
            var r = await Task.WhenAll(exchangeLoadTasks);
            Console.WriteLine($"Загрузили за {timer.Result}");
            //var start_edit = r.ToDictionary(rr => rr.Exchange.Settings.Print(), rr => rr.InfoCryptos.Count);

            

            #region ЗАПИСЬ ИНФОРМАЦИИ С БИРЖ
            string path = Directory.GetCurrentDirectory();
            string pairsFolder = Path.Combine(path, "Pairs");
            string pairsCountFilePath = Path.Combine(pairsFolder, $"pairCount.json");

            if (!Directory.Exists(pairsFolder))
                Directory.CreateDirectory(pairsFolder);

            var statistics = AnalyzeCoinPairs(r);
            statistics = statistics.OrderByDescending(x => x.Value).ToDictionary();

            List<dynamic> statWithId = new();
            int x = 0;
            foreach (var item in statistics)
            {
                statWithId.Add(new
                {
                    Id = x,
                    Pair = item.Key,
                    Count = item.Value
                });

                x++;
            }

            string statisticsJson = JsonConvert.SerializeObject(statWithId);

            await File.WriteAllTextAsync(pairsCountFilePath , statisticsJson);
            Console.WriteLine($"Файл co статистикой записан по пути {pairsCountFilePath}");

            #region ЗАПИСЬ ПАР С БИРЖ
            // foreach (var item in r)
            // {
            //     string json = JsonConvert.SerializeObject(item);

            //     // await File.WriteAllTextAsync(pairsFolder, json);
            //     // Console.WriteLine($"Файл для биржи {item.Exchange} записан по пути {pairsFolder}");

            //     string filePath = Path.Combine(pairsFolder, $"{item.Exchange}.json");

            //     await File.WriteAllTextAsync(filePath, json);
            //     Console.WriteLine($"Файл для биржи {item.Exchange} записан по пути {filePath}");
            // }
            #endregion
            #endregion




            // //фильтруем
            // int sum1 = r.Sum(x => x.InfoCryptos.Count);
            // if(LaunchConfig.ExchangeAssets.Count > 1)
            //     PairHelper.FilterPair(r);
            // for (int i = 0; i < r.Length; i++)
            //     r[i].InfoCryptos = FilterSingle(r[i].InfoCryptos);
            // int sum2 = r.Sum(x => x.InfoCryptos.Count);
            // Console.WriteLine($"Фильтр: {sum1} -> {sum2}");

            // //for (int i = 0; i < r.Length; i++)
            // //{
            // //    var item = r[i];
            // //    item.InfoCryptos.Shuffle();
            // //}

            // // Этап 2: Инициализация подключений и создание криптовалют
            // var initTasks = r.Select(async item =>
            // {
            //     try
            //     {
            //         MyTimer timer = new MyTimer();

            //         await item.Exchange.Init(item.InfoCryptos.Count);
            //         var localCryptos = await CreateCryptosAsync(item.Exchange, item.InfoCryptos, 100);
            //         lock (Cryptos)
            //             Cryptos.AddRange(localCryptos);

            //         Console.WriteLine($"[{item.Exchange.Settings.Print()}] Инициализация завершена за {timer.Result}");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"[{item.Exchange.Settings.Print()}] Ошибка инициализации: {ex.Message}");
            //     }
            // });

            // await Task.WhenAll(initTasks);
        }

        public static Dictionary<string, int> AnalyzeCoinPairs(dynamic exchangeData)
        {
            // var statistics = new Dictionary<string, Dictionary<string, int>>();
            var statistics = new Dictionary<string, int>();

            foreach (var exchange in exchangeData)
            {
                foreach (var pair in exchange.Item2)
                {

                    string baseCoin = pair.BaseCoin;
                    string quoteCoin = pair.QuoteCoin;

                    if (string.IsNullOrEmpty(baseCoin)) continue;
                    if (string.IsNullOrEmpty(quoteCoin)) continue;

                    string newPair = $"{baseCoin}-{quoteCoin}-{exchange.Item3}";

                    if (!statistics.ContainsKey(newPair))
                    {
                        statistics[newPair] = 0;
                    }

                    statistics[newPair]++;
                }
            }

            return statistics;
        }

        public static Exchange CreateExchange(ExchangeAssetInfo settings)
        {
            return settings.Name switch
            {
                ExchangeEnum.ByBit => new ByBitAPI(settings),
                ExchangeEnum.KuCoin => new KuCoinAPI(settings),
                ExchangeEnum.Mexc => new MexcAPI(settings),
                ExchangeEnum.Gateio => new GateioAPI(settings),
                ExchangeEnum.LBank => new LBankAPI(settings),
                ExchangeEnum.HTX => new HTXAPI(settings),
                _ => throw new NotImplementedException()
            };
        }
        private List<CoinInfo> FilterSingle(List<CoinInfo> coins)
        {
            coins = coins.Where(x => x.QuoteCoin == "USDT").ToList();

            #region test
            if (LaunchConfig.Coins.Count > 0)
                coins = coins.Where(x => LaunchConfig.Coins.Contains(x.BaseCoin.ToUpper())).ToList();
            if (LaunchConfig.IgnoreCoins.Count > 0)
                coins = coins.Where(x => !LaunchConfig.IgnoreCoins.Contains(x.BaseCoin.ToUpper())).ToList();
            if (LaunchConfig.CoinsMax > 0)
            {
                coins = coins.Take(LaunchConfig.CoinsMax).ToList();
                var tickerM = string.Join(", ", coins.Select(x => x.BaseCoin));
                Console.WriteLine(tickerM);
            }
            #endregion

            return coins;
        }
        private async Task<List<Crypto>> CreateCryptosAsync(Exchange exchange, List<CoinInfo> coins, int maxConcurrency)
        {
            var result = new List<Crypto>();
            var semaphore = new SemaphoreSlim(maxConcurrency);
            int total = coins.Count;
            int completed = 0;
            bool halfLogged = false;

            var tasks = coins.Select(async coin =>
            {
                await semaphore.WaitAsync();

                try
                {
                    MyTimer t = new();
                    var crypto = new Crypto();
                    var cryptoAsset = new CryptoAssetInfo(coin, exchange.Settings.AssetType);
                    await crypto.Create(exchange, cryptoAsset);

                    lock (result)
                        result.Add(crypto);

                    int done = Interlocked.Increment(ref completed);
                    if (!halfLogged && done >= total / 2)
                    {
                        lock (semaphore)
                        {
                            if (!halfLogged && done >= total / 2)
                            {
                                halfLogged = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки {coin.BaseCoin}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            return result;
        }

        public async Task Connect()
        {
            if (LaunchConfig.PriceEnable)
            {
                Console.WriteLine("Подключение к сокетам..");
                //foreach (var exchange in Exchanges)
                //{
                //    //await Task.Delay(1000);
                //    await exchange.ConnectBook();
                //}
                var tasks_connect = Exchanges.Select(exchange => exchange.ConnectBook()).ToList();
                await Task.WhenAll(tasks_connect);
            }
        }
    }
}

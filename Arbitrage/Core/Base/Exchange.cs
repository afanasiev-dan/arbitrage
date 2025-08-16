using Arbitrage.Core.Base.Enums;
using Arbitrage.Other;

namespace Arbitrage.Service.Base
{
    public abstract class Exchange
    {
        protected static HttpClient client = new HttpClient();
        public ExchangeAssetInfo Settings;
        public List<SocketBook> sockets = new();

        private readonly SemaphoreSlim _rateLimitSemaphore = new(1, 1);
        private DateTime _lastRequestTime = DateTime.UtcNow;
        private int _delayMsBetweenRequests => Settings.LimitResponse; // (50) = 20 запросов в секунду

        public Exchange(ExchangeAssetInfo settings)
        {
            Settings = settings;
        }

        public async virtual Task Init(int size)
        {
            int k = 1;
            if (Settings.Socket.WsCap != 0)
                k = Math.Max(1, (int)Math.Ceiling((float)size / Settings.Socket.WsCap));
            for (int i = 0; i < k; i++)
            {
                var new_socket = CreateSocketBook();
                new_socket.Init(Settings, $"{i}");
                sockets.Add(new_socket);
            }
            Console.WriteLine($"[{Settings.Print()}] Загрузка {size} монет ({k} сокетов)");
        }
        public async Task SubscribeBookUpdates(Crypto crypto)
        {
            foreach (var socket in sockets)
                if (socket.AddToSocket(crypto))
                    break;

            if (LaunchConfig.SMAEnable)
            {
                await EnforceRateLimitAsync();
                crypto.Prices = await LoadCandles(crypto.Info.Coin.Ticker, crypto.Info.AssetType);
            }
        }

        public async Task EnforceRateLimitAsync()
        {
            if (Settings.LimitResponse == 0) return;
            await _rateLimitSemaphore.WaitAsync();

            try
            {
                var now = DateTime.UtcNow;
                var diff = now - _lastRequestTime;

                if (diff.TotalMilliseconds < _delayMsBetweenRequests)
                {
                    var wait = _delayMsBetweenRequests - (int)diff.TotalMilliseconds;
                    if (wait > 0)
                        await Task.Delay(wait);
                }

                _lastRequestTime = DateTime.UtcNow;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }
        public virtual async Task<Dictionary<DateTime, decimal>> LoadCandles(string ticker, AssetTypeEnum assetType)
        {
            switch (assetType)
            {
                case AssetTypeEnum.Spot:
                    return await LoadSpotCandles(ticker, assetType);
                case AssetTypeEnum.Futures:
                    return await LoadFutureCandles(ticker, assetType);
                default:
                    return null;
            }
        }
        public abstract Task<Dictionary<DateTime, decimal>> LoadSpotCandles(string ticker, AssetTypeEnum assetType);
        public abstract Task<Dictionary<DateTime, decimal>> LoadFutureCandles(string ticker, AssetTypeEnum assetType);

        public async Task<List<CoinInfo>> GetCoins(AssetTypeEnum type)
        {
            try
            {
                switch (type)
                {
                    case AssetTypeEnum.Spot:
                        return await GetSpotCoins();
                    case AssetTypeEnum.Futures:
                        return await GetFutureCoins();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Settings.Print()} {ex.Message}");
                return null;
            }
        }

        protected abstract Task<List<CoinInfo>> GetSpotCoins();
        protected abstract Task<List<CoinInfo>> GetFutureCoins();
        public abstract SocketBook CreateSocketBook();
        public async Task ConnectBook()
        {
            var dt = DateTime.Now;

            //var connectTasks = sockets.Select(ws => ws.ConnectAsync());
            //foreach (var socket in sockets)
            //{
            //    await Task.Delay(100);
            //    await socket.ConnectAsync();
            //}
            //await Task.WhenAll(connectTasks);

            var connectTasks = sockets
                .Select(async socket =>
                {
                    await socket.ConnectAsync();
                });

            await Task.WhenAll(connectTasks);

            Console.WriteLine($"[{Settings.Print()}] {(DateTime.Now - dt).TotalMilliseconds}");
        }

        public abstract Task UpdateFunding();
        public Crypto GetCrypto(string ticker)
        {
            return sockets.SelectMany(s => s.updateDict.Values)
                          .FirstOrDefault(c => c.Info.Coin.Ticker == ticker);
        }
    }
}

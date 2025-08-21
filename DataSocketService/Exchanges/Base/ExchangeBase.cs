using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Model;
using DataSocketService.Other;

namespace DataSocketService.Exchanges.Base
{
    //ExchangeClient = 1 биржа
    //Sockets = 1 биржу делим на несколько групп (1000 подписок на одном сокете нельзя)
    public abstract class ExchangeBase
    {
        public (string Name, MarketType Type) Info;
        public string Name => FormatUtils.ExchangeToStr(Info.Name, Info.Type);

        public SocketSettings Settings => LaunchConfig.SocketSettings[Name];
        public List<SocketBase> Sockets = new();
        private readonly Func<SocketBase> _socketCreator;

        public ExchangeBase(string exchangeName, MarketType exchangeType, List<CurrencyPairResponceDto> currencyPairs, Func<SocketBase> socketCreator)
        {
            Info = (exchangeName, exchangeType);
            _socketCreator = socketCreator ?? throw new ArgumentNullException(nameof(socketCreator));

            int size = currencyPairs.Count;
            int k = 1;
            if (Settings.WsCap != 0)
                k = Math.Max(1, (int)Math.Ceiling((float)size / Settings.WsCap));

            for (int i = 0; i < k; i++)
            {
                var new_socket = CreateSocketBook();
                new_socket.Init(this, $"{i}");
                Sockets.Add(new_socket);
            }
            Console.WriteLine($"[{Name}] Загрузка {size} монет ({k} сокетов)");

            foreach (var pair in currencyPairs)
                SubscribeBookUpdates(pair);
        }

        public abstract SocketBase CreateSocketBook();
        protected SocketBase CreateSocket() => _socketCreator();

        public List<CurrencyPairBook> GetAllBooks()
        {
            return Sockets.SelectMany(s => s.currencyPairDict.Values).ToList();
        }

        public void SubscribeBookUpdates(CurrencyPairResponceDto currencyPair)
        {
            foreach (var socket in Sockets)
                if (socket.AddToSocket(currencyPair))
                    break;
        }
        public async Task ConnectBook()
        {
            var dt = DateTime.Now;

            var connectTasks = Sockets
                .Select(async socket =>
                {
                    await socket.ConnectAsync();
                });

            await Task.WhenAll(connectTasks);

            Console.WriteLine($"[{Name}] {(DateTime.Now - dt).TotalMilliseconds}");
        }
    }

}

using Arbitrage.ExchangeDomain;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Symbols.Domain.Entities;
using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;
using DataSocketService.Model;
using DataSocketService.Other;
using DataSocketService.Service.Exchan.ByBit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Service
{
    //ExchangeClient = 1 биржа
    //Sockets = 1 биржу делим на несколько групп (1000 подписок на одном сокете нельзя)
    public abstract class ExchangeBase
    {
        public (string Name, MarketType Type) Info;
        public string Name => F.ExchangeToStr(Info.Name, Info.Type);

        public SocketSettings Settings => LaunchConfig.SocketSettings[Name];
        public List<SocketBase> Sockets = new();

        public ExchangeBase(string exchangeName, MarketType exchangeType, List<CurrencyPairResponceDto> currencyPairs)
        {
            Info = (exchangeName, exchangeType);

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

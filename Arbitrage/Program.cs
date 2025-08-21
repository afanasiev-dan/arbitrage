using Arbitrage.Core.App;
using Arbitrage.Other.Telegram;
using Arbitrage.Service.Base;
using System.Collections.Concurrent;

namespace ArbitrageSignalBot
{
    public class Program
    {
        public static ExchangeManager ExchangeManager;
        public static SignalProcessor SignalProcessor;
        public static DateTime TimeStart;

        public static async Task Main(string[] args)
        {
            ThreadPool.GetMinThreads(out int work, out int io);
            ThreadPool.SetMinThreads(Environment.ProcessorCount * 2, io);

            try
            {
                ExchangeManager = new ExchangeManager();
                await ExchangeManager.Start();

                //var signalManager = new SignalManager("Data/signals.json");
                //var telegramBot = new TelegramBot(signalManager);
                //var fundingUpdater = new FundingUpdater(ExchangeManager);
                SignalProcessor = new SignalProcessor(signalManager, telegramBot, ExchangeManager.Cryptos);

                //signalManager.LoadSignals();

                await ExchangeManager.Connect();
                TimeStart = DateTime.Now;
                Console.WriteLine($"ЗАПУСК {TimeStart}");

                //fundingUpdater.Start();
                //await fundingUpdater.WaitForInitializationAsync();
                SignalProcessor.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Launch] {ex.Message}");
            }

            Console.ReadKey();
        }

        public static void PrintSignalByBtn(long chatId, Signal signal)
        {
            try
            {
                var f_signal = SignalProcessor.signals[chatId].Where(x => x.Id == signal.Id).FirstOrDefault();
                if (f_signal != null)
                    SignalProcessor.telegramBot.ActivateSignal(chatId, null, f_signal, stateSignal: StateSignal.Auto);
                else
                    SignalProcessor.telegramBot.SendMessage((int)chatId, "не найдено");
            }
            catch (Exception ex)
            {
                SignalProcessor.telegramBot.SendMessage((int)chatId, "не найдено");
                Console.WriteLine($"[print] {ex.Message}");
            }
        }

        public static string GetInfoCoin(string baseCoin)
        {
            List<Crypto> cryptos_f = new();
            foreach (var exchange in ExchangeManager.Exchanges)
            {
                foreach (var socket in exchange.sockets)
                {
                    var matchingCrypto = socket.updateDict.Values
                        .FirstOrDefault(c => c.Info.Coin.BaseCoin.Equals(baseCoin, StringComparison.OrdinalIgnoreCase));
                    if (matchingCrypto != null)
                    {
                        cryptos_f.Add(matchingCrypto);
                    }
                }
            }

            string m = $"Монета {baseCoin.ToUpper()}:\n\n" +
                       string.Join("\n", cryptos_f.Select(c => c.PrintNice()));
            return m;
        }

        public static string GetInfoPair(string baseCoin)
        {
            var pairs = SignalProcessor.pairs.Where(x => x.LongPos.Info.Coin.BaseCoin.Equals(baseCoin, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Result);
            string m = $"Монета {baseCoin.ToUpper()}:\n"
                + string.Join("\n", pairs.Select(x => $"{x.Print()} Life:{x.LifeTimeSec}"));
            return m;
        }
    }
}
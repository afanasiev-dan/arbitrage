using Arbitrage.Other.Telegram;
using ArbitrageSignalBot;
using Newtonsoft.Json;

namespace Arbitrage.Service.Base
{
    public class SignalManager
    {
        private readonly string _filePath;
        public Dictionary<long,Settings> settings = new();

        public SignalManager(string filePath)
        {
            _filePath = filePath;
        }

        public void AddSignal(long chatId, PreSignal signal)
        {
            if (!settings.ContainsKey(chatId))
                settings.Add(chatId, new Settings());

            settings[chatId].madeSignals.Add(signal);
            Save();
        }

        public void ChangeVolume(long chatId, decimal volume)
        {
            if (!settings.ContainsKey(chatId))
                settings.Add(chatId, new Settings());

            settings[chatId].volume = volume;
            Save();
        }
        public void ChangeProfit(long chatId, decimal profit)
        {
            if (!settings.ContainsKey(chatId))
                settings.Add(chatId, new Settings());

            settings[chatId].profit = profit;
            Save();
        }      
        //public void ChangeSlippage(long chatId, decimal value)
        //{
        //    if (!settings.ContainsKey(chatId))
        //        settings.Add(chatId, new Settings());

        //    settings[chatId].slippage = value;
        //    Save();
        //}
        public void ChangeInterval(long chatId, int inerval)
        {
            if (!settings.ContainsKey(chatId))
                settings.Add(chatId, new Settings());

            settings[chatId].interval = inerval;
            Save();
        }
        public void ChangeStateBot(long chatId, bool new_state)
        {
            if (!settings.ContainsKey(chatId))
                settings.Add(chatId, new Settings());

            settings[chatId].isWork = new_state;
            Save();
        }

        public bool RemoveSignal(long chatId, string id)
        {
            if (settings.ContainsKey(chatId))
            {
                var signal = settings[chatId].madeSignals.Find(x => x.Id == id);
                if (signal != null)
                {
                    settings[chatId].madeSignals.Remove(signal);
                    Program.SignalProcessor.telegramBot.ClearSignal(id);
                    Save();
                    return true;
                }
            }
            return false;
        }

        public List<PreSignal> GetSignals(long chatId)
        {
            if (settings.ContainsKey(chatId))
                return settings[chatId].madeSignals;
            else
                return null;
        }

        public Settings GetSettings(long chatId)
        {
            if (settings.ContainsKey(chatId))
                return settings[chatId];
            else
                return null;
        }
        public void CreateSettings(long chatId)
        {
            if (settings.ContainsKey(chatId))
                return;
            else
            {
                settings[chatId] = new Settings();
                Save();
            }
        }

        public void LoadSignals()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                settings = JsonConvert.DeserializeObject<Dictionary<long,Settings>>(json);
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Settings
    {
        public List<PreSignal> madeSignals = new();
        public int interval = 10;
        public decimal volume = 10;
        public decimal profit = 1;
        //public decimal slippage = 0.4m;
        public bool isWork = true;

        [JsonIgnore]
        public State _currentState;
        [JsonIgnore]
        public PreSignal _currentSignal;
    }
}

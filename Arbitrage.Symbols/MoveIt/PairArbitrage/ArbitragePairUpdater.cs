//using Arbitrage.Test.CoinNames;
//using Newtonsoft.Json;

//namespace Arbitrage.Symbols.MoveIt.PairArbitrage
//{
//    public class ArbitragePairUpdater
//    {
//        public static async Task Update()
//        {
//            string jsonInput = File.ReadAllText("test/marketPairs.json");
//            List<MarketPair> marketPairs = JsonConvert.DeserializeObject<List<MarketPair>>(jsonInput);
//            List<ArbPair> arbPairs = ArbitragePairHelper.Create(marketPairs);
//            string jsonOutput = JsonConvert.SerializeObject(arbPairs, Formatting.Indented);
//            File.WriteAllText("test/arbPairs.json", jsonOutput);
//        }
//    }
//}

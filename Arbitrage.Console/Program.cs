using Arbitrage.Symbols;
using Arbitrage.Symbols.MoveIt.PairArbitrage;
using Arbitrage.Test;

await LoadSymbols.Execute();
await LoadExchanges.Execute();
await LoadSymbolPairs.Execute();
// await MarketPairUpdater.Update();
// await ArbitragePairUpdater.Update();
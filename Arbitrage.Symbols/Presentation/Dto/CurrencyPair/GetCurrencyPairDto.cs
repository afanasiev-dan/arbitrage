namespace Arbitrage.Symbols.Presentation.Dto.CurrencyPair
{
    public class GetCurrencyPairDto
    {
        
        /// <summary>
        /// Название монеты лонга 
        /// </summary>
        public string BaseCoinName { get; set; }

        /// <summary>
        /// Название монеты шорта 
        /// </summary>
        public string? QuoteCoinName { get; set; } = "USDT";

    }
}
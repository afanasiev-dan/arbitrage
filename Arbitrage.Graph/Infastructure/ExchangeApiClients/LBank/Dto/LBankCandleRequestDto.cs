namespace Arbitrage.Graph.Infastructure.ExchangeApiClients.LBank.Dto
{
    public class LBankCandleResponceDto : CandleResponceDtoBase
    {
        // [JsonConstructor]
        // public LBankCandleResponceDto (decimal[] array)
        // {
        //     if (array == null || array.Length < 6)
        //         throw new ArgumentException("Invalid candle data");

        //     OpenTime = (long)array[0];
        //     Open = array[1].ToString();
        //     High = array[2].ToString();
        //     Low = array[3].ToString();
        //     Close = array[4].ToString();
        //     Volume = array[5].ToString();
        // }
    }
}
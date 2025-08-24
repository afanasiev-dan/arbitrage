using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Graph.Domain.Entities
{
    public class CandleBase
    {

        /// <summary>
        /// Индификатор свечи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Id биржи
        /// </summary>
        public Guid ExchangeId { get; set; }

        /// <summary>
        /// Id торговой пары 
        /// </summary>
        public Guid CurrencyPairId { get; set; }

        /// <summary>
        /// Время открытия
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Интвервал свечи
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Цена открытия
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// Максимальная цена
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Минимальная цена
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// Цена закрытия
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// Тип маркета
        /// </summary>
        public string? MarketType { get; set; }

        /// <summary>
        /// Название биржи
        /// </summary>
        public ExchangeModel? Exchange { get; set; }

        /// <summary>
        /// Торговая пара
        /// </summary>
        public CurrencyPair? Pair { get; set; }
    }
}
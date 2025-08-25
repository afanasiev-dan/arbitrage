using System.ComponentModel.DataAnnotations.Schema;
using Arbitrage.Exchange.Domain.Entities;
using Arbitrage.Symbols.Domain.Entities;

namespace Arbitrage.Graph.Domain.Entities
{
    [Table("ArbitrageCandle")]
    public class ArbitrageCandle
    {
        /// <summary>
        /// Индификатор свечи
        /// </summary>
        public Guid Id { get; set; }

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
        /// Id монеты лонга
        /// </summary>
        public Guid BaseCoinId { get; set; }

        /// <summary>
        /// Id монеты шорта
        /// </summary>
        public Guid QuoteCoinId { get; set; }

        /// <summary>
        /// Id биржи лонга
        /// </summary>
        public Guid ExchangeLongId { get; set; }

        /// <summary>
        /// Тип маркета лонга
        /// </summary>
        public string? MarketTypeLong { get; set; }

        /// <summary>
        /// Id биржи шорта
        /// </summary>
        public Guid ExchangeShortId { get; set; }

        /// <summary>
        /// Тип маркета шорта
        /// </summary>
        public string? MarketTypeShort { get; set; }

        /// <summary>
        /// Объект монеты
        /// </summary>
        public Coin BaseCoin { get; set; }

        /// <summary>
        /// Объект монеты
        /// </summary>
        public Coin QuoteCoin { get; set; }

        /// <summary>
        /// Объект биржи
        /// </summary>
        public ExchangeModel? ExchangeLong { get; set; }

        /// <summary>
        /// Объект биржи
        /// </summary>
        public ExchangeModel? ExchangeShort { get; set; }
    }
}
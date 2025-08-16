using System.ComponentModel.DataAnnotations;
using Arbitrage.ExchangeDomain.Enums;
using Arbitrage.Graph.Presentation.Validations;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Arbitrage.Graph.Presentation.Dto;

public class SpreadCandlesRequestDto : IValidatableObject
{
    /// <summary>
    /// Дата для первой свечи
    /// </summary>
    [UtcDateTime]
    public DateTime DateFrom { get; set; }

    /// <summary>
    /// Дата для последней свечи
    /// </summary>
    [UtcDateTime]
    public DateTime DateTo { get; set; }

    /// <summary>
    /// Название биржи лонга
    /// </summary>
    public string ExchangeNameLong { get; set; }
    public MarketType MarketTypeLong { get; set; }

    /// <summary>
    /// Название монеты лонга 
    /// </summary>
    public string SymbolNameLong { get; set; }

    /// <summary>
    /// Название биржи шорта 
    /// </summary>
    public string ExchangeNameShort { get; set; }
    public MarketType MarketTypeShort { get; set; } = MarketType.Futures;

    /// <summary>
    /// Название символа по стандарту USDT
    /// </summary>
    public string SymbolNameShort { get; set; } = "USDT";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExchangeNameLong == string.Empty || ExchangeNameShort == string.Empty || SymbolNameLong == string.Empty || SymbolNameShort == string.Empty)
        {
            yield return new ValidationResult("Биржи или монеты не заполнены", new[] { nameof(ExchangeNameLong), nameof(ExchangeNameShort), nameof(SymbolNameLong), nameof(SymbolNameShort) });
        }
    }
}
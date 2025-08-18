using System.ComponentModel.DataAnnotations;
using Arbitrage.ExchangeDomain.Enums;

namespace Arbitrage.Scaner.Presentation.Dto
{
    public class ScanerAddDataRequestDto : IValidatableObject
    {
        public string BaseCoinName { get; set; }
        public string QuoteCoinName { get; set; }

        public string ExchangeNameLong { get; set; }
        public MarketType MarketTypeLong { get; set; }
        public decimal PurchasePriceLong { get; set; }
        public decimal FundingRateLong { get; set; }


        public string ExchangeNameShort { get; set; }
        public MarketType MarketTypeShort { get; set; }
        public decimal PurchasePriceShort { get; set; }
        public decimal FundingRateShort { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = [];

            if (string.Empty == BaseCoinName ||
             string.Empty == QuoteCoinName ||
             string.Empty == ExchangeNameLong ||
             string.Empty == ExchangeNameShort)
            {
                errors.Add(new ValidationResult("Базовая монета, котируемая монета, и названия бирж не должны быть пустыми"));
            }

            return errors;
        }
    }
}
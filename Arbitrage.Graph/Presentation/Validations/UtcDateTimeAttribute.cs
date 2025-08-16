using System.ComponentModel.DataAnnotations;

namespace Arbitrage.Graph.Presentation.Validations
{
    public class UtcDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Kind != DateTimeKind.Utc)
                {
                    return new ValidationResult($"Ошибка валидации, {validationContext.DisplayName} поле должно быть в формате UTC");
                }
            }

            return ValidationResult.Success;
        }
    }
}
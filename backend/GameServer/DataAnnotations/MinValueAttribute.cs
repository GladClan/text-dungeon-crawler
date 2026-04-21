using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Gameserver.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MinimumValueAttribute : ValidationAttribute
{
    private readonly double _min;

    public MinimumValueAttribute(double min)
    {
        _min = min;
        ErrorMessage = $"Value must be greater than or equal to {_min}.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (double.TryParse(
                Convert.ToString(value, CultureInfo.InvariantCulture),
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var result) && result > _min)
        {
            return ValidationResult.Success;
        }
        
        return new ValidationResult(ErrorMessage);
    }
}
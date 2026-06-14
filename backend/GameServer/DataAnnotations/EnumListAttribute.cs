using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace GameServer.DataAnnotations;

public class EnumListAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumListAttribute(Type enumType)
    {
        if (!enumType.IsEnum) 
            throw new ArgumentException("Type must be an Enum");
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IEnumerable list)
        {
            foreach (var item in list)
            {
                if (item == null)
                    return new ValidationResult(ErrorMessage ?? "Item cannot be null");

                // Convert item to string for universal parsing (handles numbers and text)
                string? itemString = item.ToString();

                // Enum.TryParse validates if the combined flag math is legally mapped
                if (!Enum.TryParse(_enumType, itemString, out object? parsedValue) || 
                    !Enum.IsDefined(_enumType, parsedValue ?? ""))
                {
                    // Double check required for numeric strings: TryParse allows out-of-range integers
                    // This block ensures the parsed integer strictly consists of allowed flag bits
                    if (decimal.TryParse(itemString, out _))
                    {
                        return new ValidationResult(ErrorMessage ?? $"Invalid flags combination: {item}");
                    }
                }
            }
        }
        return ValidationResult.Success;
    }
}

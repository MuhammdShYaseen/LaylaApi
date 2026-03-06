using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Attributes
{
    public sealed class NotWhiteSpaceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string str &&
                string.IsNullOrWhiteSpace(str))
            {
                return new ValidationResult(
                    "Value cannot be empty or whitespace");
            }

            return ValidationResult.Success;
        }
    }
}

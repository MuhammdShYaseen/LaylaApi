using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Attributes
{
    public sealed class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxSize;

        public MaxFileSizeAttribute(long maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is not IFormFile file)
                return ValidationResult.Success;

            if (file.Length > _maxSize)
            {
                return new ValidationResult(
                    $"Max file size is {_maxSize / 1024 / 1024} MB");
            }

            return ValidationResult.Success;
        }
    }
}

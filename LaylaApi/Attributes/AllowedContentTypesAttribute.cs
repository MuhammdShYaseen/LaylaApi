using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Attributes
{
    public sealed class AllowedContentTypesAttribute : ValidationAttribute
    {
        private readonly string[] _types;

        public AllowedContentTypesAttribute(params string[] types)
        {
            _types = types;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is not IFormFile file)
                return ValidationResult.Success;

            if (!_types.Contains(file.ContentType))
            {
                return new ValidationResult(
                    "Invalid file type");
            }

            return ValidationResult.Success;
        }
    }
}

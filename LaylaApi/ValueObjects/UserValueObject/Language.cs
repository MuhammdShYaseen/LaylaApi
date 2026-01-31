using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Services.LanguageServices;

namespace LaylaApi.ValueObjects.UserValueObject
{
    public class Language : ValueObject
    {
       
        public string Code { get; }

        private Language(string code)
        {
            Code = code;
        }

        public static Language Create(string code, ISupportedLanguagePolicy policy)
        {
            Validate(code, policy);

            code = code.Trim().ToLowerInvariant();

            return new Language(code);
        }
        internal static Language FromPersistence(string code)
        {
            return new Language(code);
        }

        private static void Validate(string code, ISupportedLanguagePolicy policy)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Language is required.");

            if (!policy.IsSupported(code))
                throw new ArgumentException("Unsupported language.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        public override string ToString() => Code;
    }
}

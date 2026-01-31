using LaylaApi.Options;
using Microsoft.Extensions.Options;

namespace LaylaApi.Services.LanguageServices
{
    public class SupportedLanguagePolicy : ISupportedLanguagePolicy
    {
        private readonly HashSet<string> _languages;

        public SupportedLanguagePolicy(IOptions<LanguageOptions> options)
        {
            _languages = options.Value.SupportedLanguages
                .Select(l => l.ToLowerInvariant())
                .ToHashSet();
        }

        public bool IsSupported(string languageCode)
            => _languages.Contains(languageCode);
    }
}

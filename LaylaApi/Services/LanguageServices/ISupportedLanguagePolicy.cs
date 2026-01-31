namespace LaylaApi.Services.LanguageServices
{
    public interface ISupportedLanguagePolicy
    {
        public bool IsSupported(string languageCode);
    }
}

using System.Globalization;

namespace LaylaApi.Helper.Localization
{
    public static class LocalizationHelper
    {
        public static IDisposable UseCulture(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
                culture = "en";

            var originalCulture = CultureInfo.CurrentCulture;
            var originalUICulture = CultureInfo.CurrentUICulture;

            var newCulture = new CultureInfo(culture);

            CultureInfo.CurrentCulture = newCulture;
            CultureInfo.CurrentUICulture = newCulture;

            return new CultureScope(originalCulture, originalUICulture);
        }

        private sealed class CultureScope : IDisposable
        {
            private readonly CultureInfo _originalCulture;
            private readonly CultureInfo _originalUICulture;

            public CultureScope(CultureInfo culture, CultureInfo uiCulture)
            {
                _originalCulture = culture;
                _originalUICulture = uiCulture;
            }

            public void Dispose()
            {
                CultureInfo.CurrentCulture = _originalCulture;
                CultureInfo.CurrentUICulture = _originalUICulture;
            }
        }
    }
}

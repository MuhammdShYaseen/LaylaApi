using LaylaApi.Services.DataCRUD.Interfaces;
using System.Globalization;

namespace LaylaApi.Middleware.Localization
{
    public class UserLanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public UserLanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = int.Parse(context.User.FindFirst("id")!.Value);
                var lang = await userService.GetUserPreferredLanguage(userId);

                if (!string.IsNullOrEmpty(lang))
                {
                    var culture = new CultureInfo(lang);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
            }

            await _next(context);
        }
    }
}

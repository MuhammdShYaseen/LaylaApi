using Microsoft.Extensions.Localization;

namespace LaylaApi.Services.LocalizationServieces.MessagesLocalizerService
{
    public class MessagesLocalizer : IMessagesLocalizer
    {
        private readonly IStringLocalizerFactory _factory;
        private readonly string _basePath = "Localization.Messages";
        private readonly IStringLocalizer _sharedLocalizer;
        public MessagesLocalizer(IStringLocalizerFactory factory, IStringLocalizer sharedLocalizer)
        {
            _factory = factory;
            var type = typeof(MessagesLocalizer);
            _sharedLocalizer = factory.Create("SharedMessages", type.Assembly.FullName!);
        }
        public IStringLocalizer Get(string moduleName, string group = "")
        {
            string fullBaseName;

            if (!string.IsNullOrEmpty(group))
            {
                // مثال: Localization.Messages.FromServices.AdminDashboard.Messages
                fullBaseName = $"{_basePath}.{group}.{moduleName}.Messages";
            }
            else
            {
                // مثال: Localization.Messages.Apartments.Messages
                fullBaseName = $"{_basePath}.{moduleName}.Messages";
            }

            return _factory.Create(fullBaseName,
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!);
        }

        public string Localize(string key, params object?[]? args)
        {
            var result = _sharedLocalizer[key, args!];

            // إذا لم يجد المفتاح → يرجع المفتاح نفسه كـ fallback
            if (result.ResourceNotFound)
                return key;

            return result.Value ?? key;
        }
    }
}

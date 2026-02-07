using LaylaApi.Attributes;
using LaylaApi.Models.DtosModels.MainDtos;
using System.Linq.Expressions;
using System.Reflection;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public static class SortRegistry
    {
        private static readonly Dictionary<string, LambdaExpression> _map;

        static SortRegistry()
        {
            _map = typeof(ApartmentSortMap)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.IsDefined(typeof(SortableAttribute)))
                .ToDictionary(
                    p => p.Name.ToLowerInvariant(),
                    p => (LambdaExpression)p.GetValue(null)!
                );
        }

        public static bool TryGet(string key, out LambdaExpression exp)
            => _map.TryGetValue(key.ToLowerInvariant(), out exp!);
    }
}

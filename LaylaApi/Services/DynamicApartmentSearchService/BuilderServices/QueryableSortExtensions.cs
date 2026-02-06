using LaylaApi.Models.DtosModels.MainDtos;
using System.Linq.Expressions;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public static class QueryableSortExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, ApartmentSearchRequestDto.SortDirections direction)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var param = Expression.Parameter(typeof(T));
            var property = Expression.Property(param, sortBy);

            var lambda = Expression.Lambda(property, param);

            var method = direction == ApartmentSearchRequestDto.SortDirections.Asc
                ? "OrderBy"
                : "OrderByDescending";

            var call = Expression.Call(
                typeof(Queryable),
                method,
                new[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(call);
        }
    }
}

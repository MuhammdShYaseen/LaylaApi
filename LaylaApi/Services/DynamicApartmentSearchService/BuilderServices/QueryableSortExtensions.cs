using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using System.Linq.Expressions;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    internal static class QueryableSortExtensions
    {
        internal static IQueryable<Apartment> ApplySorting(this IQueryable<Apartment> query, string? sortBy, ApartmentSearchRequestDto.SortDirections direction)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query
                    .OrderByDescending(x => x.CreatedAt)
                    .ThenBy(x => x.Id);

            if (!SortRegistry.TryGet(sortBy, out var sortExp))
                return query
                    .OrderByDescending(x => x.CreatedAt)
                    .ThenBy(x => x.Id);

            var parameter = Expression.Parameter(typeof(Apartment), "x");

            var body = Expression.Invoke(sortExp, parameter);

            var lambda = Expression.Lambda(body, parameter);

            var methodName = direction == ApartmentSearchRequestDto.SortDirections.Asc
                ? "OrderBy"
                : "OrderByDescending";

            var orderByCall = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(Apartment), body.Type },
                query.Expression,
                Expression.Quote(lambda));

            var orderedQuery = query.Provider.CreateQuery<Apartment>(orderByCall);

            // ThenBy(Id) لضمان Pagination Stable
            var thenBy = Expression.Call(
                typeof(Queryable),
                "ThenBy",
                new[] { typeof(Apartment), typeof(Guid) },
                orderedQuery.Expression,
                Expression.Quote((Expression<Func<Apartment, int>>)(x => x.Id)));

            return orderedQuery.Provider.CreateQuery<Apartment>(thenBy);
        }
    }
}

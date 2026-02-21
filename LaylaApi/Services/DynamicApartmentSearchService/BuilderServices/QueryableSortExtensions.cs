using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.ValueObjects.ApartmentValueObject;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Linq.Expressions;
using static LaylaApi.Models.DtosModels.MainDtos.ApartmentSearchRequestDto;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    internal static class QueryableSortExtensions
    {
        internal static IQueryable<Apartment> ApplySorting(this IQueryable<Apartment> query, ApartmentSortBy? sortBy, SortDirections direction, Point? userLocation)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (sortBy is null)
            {
                return ApplyDefaultSort(query);
            }

            IOrderedQueryable<Apartment>? ordered = sortBy switch
            {
                ApartmentSortBy.CreatedAt =>
                    Apply(query, x => x.CreatedAt, direction),

                ApartmentSortBy.Title =>
                    Apply(query, x => x.Title!, direction),

                ApartmentSortBy.PricePerDay =>
                    Apply(query, x => x.PricePerDay!.Value, direction),

                ApartmentSortBy.PricePerHour =>
                    Apply(query, x => x.PricePerHour!.Value, direction),

                ApartmentSortBy.Area =>
                    Apply(query, x => x.Area, direction),

               ApartmentSortBy.Distance =>
                    Apply(query, x => x.Location!.Location.Distance(userLocation), direction),


                _ => null
            };

            // Fallback for any invalid value
            if (ordered is null)
            {
                return ApplyDefaultSort(query);
            }

            // Always stable pagination
            return ordered.ThenBy(x => x.Id);
        }

        private static IOrderedQueryable<Apartment> Apply<TKey>(
            IQueryable<Apartment> query,
            Expression<Func<Apartment, TKey>> selector,
            ApartmentSearchRequestDto.SortDirections direction)
        {
            return direction == ApartmentSearchRequestDto.SortDirections.Asc
                ? query.OrderBy(selector)
                : query.OrderByDescending(selector);
        }

        private static IQueryable<Apartment> ApplyDefaultSort(
            IQueryable<Apartment> query)
        {
            return query
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id);
        }

    }
}

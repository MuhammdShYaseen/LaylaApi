using LaylaApi.Attributes;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Reflection;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public class ApartmentFilterBuilder
    {
        public static Expression<Func<Apartment, bool>> Build(ApartmentSearchRequestDto request)
        {
            var predicate = PredicateBuilder.True<Apartment>();

            void Add<T>(T? value, Expression<Func<Apartment, bool>> exp, string propertyName)
            {
                if (value == null)
                    return;

                if (value is string s && string.IsNullOrWhiteSpace(s))
                    return;

                var prop = typeof(ApartmentSearchRequestDto)
                    .GetProperty(propertyName);

                var ignoreIfNonPositive =
                    prop?.IsDefined(typeof(IgnoreIfNonPositiveAttribute));

                if (ignoreIfNonPositive == true &&
                    value is IComparable c &&
                    c.CompareTo(default(T)) <= 0)
                {
                    return;
                }

                predicate = predicate.And(exp);
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var start = request.StartDate.Value;
                var end = request.EndDate.Value;
                var activeStatuses = new[]
                {
                     BookingStatus.Confirmed,
                     BookingStatus.Pending,
                     BookingStatus.Accepted
                };
                predicate = predicate.And(a =>
                    !a.Bookings!.Any(b =>
                        b.StartDate < end &&
                        b.EndDate > start &&
                        activeStatuses.Contains(b.Status)
                    ));
            }

            Add(request.MinPricePerDay, v => v.PricePerDay!.Value >= request.MinPricePerDay,nameof(request.MinPricePerDay));
            Add(request.MaxPricePerDay, v => v.PricePerDay!.Value <= request.MaxPricePerDay, nameof(request.MaxPricePerDay));

            Add(request.MinPricePerHour, v => v.PricePerHour!.Value >= request.MinPricePerHour, nameof(request.MinPricePerHour));
            Add(request.MaxPricePerHour, v => v.PricePerHour!.Value <= request.MaxPricePerHour, nameof(request.MaxPricePerHour));

            Add(request.MinArea, v => v.Area >= request.MinArea, nameof(request.MinArea));
            Add(request.MaxArea, v => v.Area <= request.MaxArea, nameof(request.MaxArea));

            Add(request.MinBedrooms, v => v.NumberOfBedRooms >= request.MinBedrooms, nameof(request.MinBedrooms));
            Add(request.MaxBedrooms, v => v.NumberOfBedRooms <= request.MaxBedrooms, nameof(request.MaxBedrooms));

            Add(request.MinBathrooms, v => v.NumberOfBathrooms >= request.MinBathrooms, nameof(request.MinBathrooms));
            Add(request.MaxBathrooms, v => v.NumberOfBathrooms <= request.MaxBathrooms, nameof(request.MaxBathrooms));

            Add(request.Type, v => v.Type == request.Type, nameof(request.Type));
            Add(request.View, v => v.View == request.View, nameof(request.View));
            Add(request.Finishing, v => v.Finishing == request.Finishing, nameof(request.Finishing));

            Add(request.HasElevator, v => v.HasElevator == request.HasElevator, nameof(request.HasElevator));
            Add(request.HasParking, v => v.HasParking == request.HasParking, nameof(request.HasParking));
            Add(request.HasPool, v => v.HasPool == request.HasPool, nameof(request.HasPool));
            Add(request.IsAvailable, v => v.IsAvailable == request.IsAvailable, nameof(request.IsAvailable));

            if (!string.IsNullOrWhiteSpace(request.Orientation))
            {
                predicate = predicate.And(a =>
                    a.Orientation == request.Orientation);
            }

            if (!string.IsNullOrWhiteSpace(request.TitleKeyword))
            {
                predicate = predicate.And(a =>
                    EF.Functions.Like(a.Title, $"%{request.TitleKeyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                predicate = predicate.And(a =>
                    EF.Functions.Like(a.Description, $"%{request.Description}%"));
            }

            if (request.UserLatitude != null && request.UserLongitude != null && request.MaxDistance.HasValue)
            {
                var lat = request.UserLatitude;
                var lon = request.UserLongitude;

                var max = request.MaxDistance.Value;
                predicate = predicate.And(a =>
                    CalculateDistanceKm(lat ?? 0, lon ?? 0, a.Location!.Location.Latitude, a.Location.Location.Longitude) <= max);
            }

            return predicate;
        }
        private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) *
                       Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) *
                       Math.Sin(dLon / 2);

            return 2 * R * Math.Asin(Math.Sqrt(h));
        }
        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}

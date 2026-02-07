using LaylaApi.Attributes;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Linq.Expressions;
using System.Reflection;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public class ApartmentFilterBuilder
    {
        private static readonly GeometryFactory _geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
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

            Add(request.MinPricePerDay, v => v.PricePerDay!.Value >= request.MinPricePerDay, nameof(request.MinPricePerDay));
            Add(request.MaxPricePerDay, v => v.PricePerDay!.Value <= request.MaxPricePerDay, nameof(request.MaxPricePerDay));

            Add(request.MinPricePerHour, v => v.PricePerHour!.Value >= request.MinPricePerHour, nameof(request.MinPricePerHour));
            Add(request.MaxPricePerHour, v => v.PricePerHour!.Value <= request.MaxPricePerHour, nameof(request.MaxPricePerHour));

            Add(request.MinArea, v => v.Area >= request.MinArea, nameof(request.MinArea));
            Add(request.MaxArea, v => v.Area <= request.MaxArea, nameof(request.MaxArea));
            Add(request.MaxFloorNumber, v => v.FloorNumber <= request.MaxFloorNumber, nameof(request.MaxFloorNumber));
            Add(request.MinBedrooms, v => v.NumberOfBedRooms >= request.MinBedrooms, nameof(request.MinBedrooms));
            Add(request.MinFloorNumber, v => v.FloorNumber >= request.MinFloorNumber, nameof(request.MinFloorNumber));
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

            if (request.UserLatitude.HasValue && request.UserLongitude.HasValue && request.MaxDistance.HasValue)
            {
                var lat = request.UserLatitude.Value;
                var lon = request.UserLongitude.Value;
                var maxKm = request.MaxDistance.Value;

                var maxMeters = maxKm * 1000;

                var latDelta = maxKm / 111.0;
                var lonDelta = maxKm / (111.0 * Math.Cos(lat * Math.PI / 180));

                var minLat = lat - latDelta;
                var maxLat = lat + latDelta;
                var minLon = lon - lonDelta;
                var maxLon = lon + lonDelta;

                var userPoint =
                    _geoFactory.CreatePoint(new Coordinate(lon, lat));

                predicate = predicate.And(a =>
                    a.Location!.Location.Y >= minLat &&
                    a.Location.Location.Y <= maxLat &&
                    a.Location.Location.X >= minLon &&
                    a.Location.Location.X <= maxLon
                );

                predicate = predicate.And(a =>
                    a.Location!.Location.IsWithinDistance(userPoint, maxMeters)
                );
            }
            return predicate;
        }
    }
}

using LaylaApi.DataAccess;
using LaylaApi.DataRepository;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Linq.Expressions;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public class ApartmentFilterBuilder : IApartmentFilterBuilder
    {
        private readonly GeometryFactory _factory;
        private readonly IRepository<Booking> _repository;
        public ApartmentFilterBuilder(GeometryFactory factory, IRepository<Booking> repository) 
        { 
            _factory = factory;
            _repository = repository;
        }
        private static readonly BookingStatus[] ActiveStatuses =
        {
            BookingStatus.Confirmed,
            BookingStatus.Pending,
            BookingStatus.Accepted
        };
        public Expression<Func<Apartment, bool>> Build(ApartmentSearchRequestDto request)
        {
            var predicate = PredicateBuilder.True<Apartment>();

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var start = request.StartDate.Value;
                var end = request.EndDate.Value;

                predicate = predicate.And(a =>
                    !_repository.Query()
                        .AsNoTracking()
                        .Where(b => b.ApartmentId == a.Id)
                        .Where(b => b.StartDate < end && b.EndDate > start)
                        .Where(b => ActiveStatuses.Contains(b.Status))
                        .Any());
            }

            if (request.MinPricePerDay > 0)
                predicate = predicate.And(a =>
                    a.PricePerDay!.Value >= request.MinPricePerDay);

            if (request.MaxPricePerDay > 0)
                predicate = predicate.And(a =>
                    a.PricePerDay!.Value <= request.MaxPricePerDay);

            if (request.MinPricePerHour > 0)
                predicate = predicate.And(a =>
                    a.PricePerHour.Value >= request.MinPricePerHour);

            if (request.MaxPricePerHour > 0)
                predicate = predicate.And(a =>
                    a.PricePerHour.Value <= request.MaxPricePerHour);

            if (request.MinArea > 0)
                predicate = predicate.And(a => a.Area >= request.MinArea);

            if (request.MaxArea > 0)
                predicate = predicate.And(a => a.Area <= request.MaxArea);

            if (request.MinFloorNumber > 0)
                predicate = predicate.And(a =>
                    a.FloorNumber >= request.MinFloorNumber);

            if (request.MaxFloorNumber > 0)
                predicate = predicate.And(a =>
                    a.FloorNumber <= request.MaxFloorNumber);

            if (request.MinBedrooms > 0)
                predicate = predicate.And(a =>
                    a.NumberOfBedRooms >= request.MinBedrooms);

            if (request.MaxBedrooms > 0)
                predicate = predicate.And(a =>
                    a.NumberOfBedRooms <= request.MaxBedrooms);

            if (request.MinBathrooms > 0)
                predicate = predicate.And(a =>
                    a.NumberOfBathrooms >= request.MinBathrooms);

            if (request.MaxBathrooms > 0)
                predicate = predicate.And(a =>
                    a.NumberOfBathrooms <= request.MaxBathrooms);

            if (request.MinLivingRooms > 0)
                predicate = predicate.And(a =>
                    a.NumberOfLivingRooms >= request.MinLivingRooms);

            if (request.MaxLivingRooms > 0)
                predicate = predicate.And(a =>
                    a.NumberOfLivingRooms <= request.MaxLivingRooms);

            if (request.Type.HasValue)
                predicate = predicate.And(a => a.Type == request.Type);

            if (request.View.HasValue)
                predicate = predicate.And(a => a.View == request.View);

            if (request.Finishing.HasValue)
                predicate = predicate.And(a => a.Finishing == request.Finishing);

            if (request.HasElevator.HasValue)
                predicate = predicate.And(a =>
                    a.HasElevator == request.HasElevator);

            if (request.HasParking.HasValue)
                predicate = predicate.And(a =>
                    a.HasParking == request.HasParking);

            if (request.HasPool.HasValue)
                predicate = predicate.And(a =>
                    a.HasPool == request.HasPool);

            if (request.IsAvailable.HasValue)
                predicate = predicate.And(a =>
                    a.IsAvailable == request.IsAvailable);

            if (!string.IsNullOrWhiteSpace(request.Orientation))
            {
                predicate = predicate.And(a =>
                    a.Orientation == request.Orientation);
            }

            if (!string.IsNullOrWhiteSpace(request.TitleKeyword))
            {
                var keyword = request.TitleKeyword.Trim();

                predicate = predicate.And(a =>
                    EF.Functions.Like(a.Title, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                var desc = request.Description.Trim();

                predicate = predicate.And(a =>
                    EF.Functions.Like(a.Description, $"%{desc}%"));
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

                var userPoint = _factory.CreatePoint(new Coordinate(lon, lat));
                userPoint.SRID = 4326;
                predicate = predicate.And(a =>
                    a.Location.Location.Y >= minLat &&
                    a.Location.Location.Y <= maxLat &&
                    a.Location.Location.X >= minLon &&
                    a.Location.Location.X <= maxLon
                );

                predicate = predicate.And(a =>
                    a.Location.Location.IsWithinDistance(userPoint, maxMeters)
                );
            }
            return predicate;
        }

    }
}

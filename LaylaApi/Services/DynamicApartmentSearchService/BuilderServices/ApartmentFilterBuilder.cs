using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.ValueObjects.ApartmentValueObject;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public class ApartmentFilterBuilder : IApartmentFilterBuilder
    {
        private readonly GeometryFactory _factory;
        public ApartmentFilterBuilder(GeometryFactory factory) 
        { 
            _factory = factory;
        }
        private static readonly BookingStatus[] ActiveStatuses =
        {
            BookingStatus.Confirmed,
            BookingStatus.Pending,
            BookingStatus.Accepted
        };
        public IQueryable<Apartment> Build(IQueryable<Apartment> query, ApartmentSearchRequestDto request)
        {
            query = query.AsNoTracking();
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var start = request.StartDate.Value;
                var end = request.EndDate.Value;

                query = query.Where(a =>
                      !a.Bookings.Any(b =>
                        b.StartDate < end &&
                        b.EndDate > start &&
                        ActiveStatuses.Contains(b.Status)));
            }

            if (request.MinPricePerDay > 0)
            {
                var minPrice = Money.Create(request.MinPricePerDay.Value);
                query = query.Where(a => a.PricePerDay != null && a.PricePerDay >= minPrice);
            }

            if (request.MaxPricePerDay > 0)
            {
                var maxPrice = Money.Create(request.MaxPricePerDay.Value);
                query = query.Where(a => a.PricePerDay != null && a.PricePerDay <= maxPrice);
            }

            if (request.MinPricePerHour > 0)
            {
                var minHourly = Money.Create(request.MinPricePerHour.Value);
                query = query.Where(a => a.PricePerHour != null && a.PricePerHour >= minHourly);
            }

            if (request.MaxPricePerHour > 0)
            {
                var maxHourly = Money.Create(request.MaxPricePerHour.Value);
                query = query.Where(a => a.PricePerHour != null && a.PricePerHour <= maxHourly);
            }

            if (request.MinArea > 0)
                query = query.Where(a => a.Area >= request.MinArea);

            if (request.MaxArea > 0)
                query = query.Where(a => a.Area <= request.MaxArea);

            if (request.MinFloorNumber > 0)
                query = query.Where(a =>
                    a.FloorNumber >= request.MinFloorNumber);

            if (request.MaxFloorNumber > 0)
                query = query.Where(a =>
                    a.FloorNumber <= request.MaxFloorNumber);

            if (request.MinBedrooms > 0)
                query = query.Where(a =>
                    a.NumberOfBedRooms >= request.MinBedrooms);

            if (request.MaxBedrooms > 0)
                query = query.Where(a =>
                    a.NumberOfBedRooms <= request.MaxBedrooms);

            if (request.MinBathrooms > 0)
                query = query.Where(a =>
                    a.NumberOfBathrooms >= request.MinBathrooms);

            if (request.MaxBathrooms > 0)
                query = query.Where(a =>
                    a.NumberOfBathrooms <= request.MaxBathrooms);

            if (request.MinLivingRooms > 0)
                query = query.Where(a =>
                    a.NumberOfLivingRooms >= request.MinLivingRooms);

            if (request.MaxLivingRooms > 0)
                query = query.Where(a =>
                    a.NumberOfLivingRooms <= request.MaxLivingRooms);

            if (request.Type.HasValue)
                query = query.Where(a => a.Type == request.Type);

            if (request.View.HasValue)
                query = query.Where(a => a.View == request.View);

            if (request.Finishing.HasValue)
                query = query.Where(a => a.Finishing == request.Finishing);

            if (request.HasElevator.HasValue)
                query = query.Where(a =>
                    a.HasElevator == request.HasElevator);

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var keyword = request.City.Trim();

                query = query.Where(a =>
                    EF.Functions.Like(a.Location!.City, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                var keyword = request.Country.Trim();

                query = query.Where(a =>
                    EF.Functions.Like(a.Location!.Country, $"%{keyword}%"));
            }

            if (request.HasParking.HasValue)
                query = query.Where(a =>
                    a.HasParking == request.HasParking);

            if (request.HasPool.HasValue)
                query = query.Where(a =>
                    a.HasPool == request.HasPool);

            if (request.IsAvailable.HasValue)
                query = query.Where(a =>
                    a.IsAvailable == request.IsAvailable);

            if (!string.IsNullOrWhiteSpace(request.Orientation))
            {
                query = query.Where(a =>
                    a.Orientation == request.Orientation);
            }

            if (!string.IsNullOrWhiteSpace(request.TitleKeyword))
            {
                var keyword = request.TitleKeyword.Trim();

                query = query.Where(a =>
                    EF.Functions.Like(a.Title, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                var desc = request.Description.Trim();

                query = query.Where(a =>
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
                query = query.Where(a =>
                    a.Location!.Location.Y >= minLat &&
                    a.Location.Location.Y <= maxLat &&
                    a.Location.Location.X >= minLon &&
                    a.Location.Location.X <= maxLon
                );

                query = query.Where(a =>
                    a.Location!.Location.IsWithinDistance(userPoint, maxMeters)
                );
            }
            return query;
        }

    }
}

using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Serilog;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ApartmentService : IApartmentService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;
        private static readonly GeometryFactory _geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        public ApartmentService(LaylaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        public async Task<IEnumerable<ApartmentDto>> GetAllAsync()
        {
            var apartments = await _context.Apartments
                  .AsNoTracking()
                  .Include(a => a.Owner)
                  .Include(a => a.MediaFiles)
                  .Include(a => a.Reviews)
                  .ToListAsync();

            return _mapper.Map<IEnumerable<ApartmentDto>>(apartments);
        }

        public async Task<ApartmentDto> GetByIdAsync(int id)
        {
            var apartment = await _context.Apartments
                .AsNoTracking()
                .Include(a => a.Owner)
                .Include(a => a.MediaFiles)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.Id == id);

            return _mapper.Map<ApartmentDto>(apartment);
        }

        public async Task<Apartment> GetEntityByIdAsync(int id)
        {
            var apartment = await _context.Apartments
                .AsNoTracking()
                .Include(a => a.Owner)
                .Include(a => a.MediaFiles)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (apartment == null)
                throw new ArgumentNullException(nameof(apartment));
            return apartment;
        }
        public async Task <IEnumerable<ApartmentDto>> GetByOwnerIdAsync(int id)
        {
            var apartments = await _context.Apartments
                  .AsNoTracking()
                  .Include(a => a.MediaFiles)
                  .Include(a => a.Reviews)
                  .Where(a => a.OwnerId == id)
                  .ToListAsync();

            return _mapper.Map<IEnumerable<ApartmentDto>>(apartments);
        }

        public async Task<IEnumerable<ApartmentDto>> SearchAsync(string keyword)
        {
            keyword = keyword.ToLower();

            var apartments = await _context.Apartments
                .AsNoTracking()
                .Include(a => a.MediaFiles)
                .Where(a =>
                    a.Title.ToLower().Contains(keyword) ||
                    a.Location!.ToString().ToLower().Contains(keyword) ||
                    (a.Description != null && a.Description.ToLower().Contains(keyword))
                )
                .ToListAsync();

            return _mapper.Map<IEnumerable<ApartmentDto>>(apartments);
        }

        public async Task<ApartmentDto> AddAsync(CreateApartmentDto dto, int userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var ownerExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!ownerExists)
                throw new KeyNotFoundException("User not found");

            var apartment = Apartment.Create(dto, userId);

            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            return _mapper.Map<ApartmentDto>(apartment);
        }

        public async Task<ApartmentDto?> UpdateAsync(int id, CreateApartmentDto dto, int ownerId, bool isAdmin)
        {
            var apartment = await _context.Apartments.FindAsync(id)?? 
                throw new KeyNotFoundException("Apartment not found.");

            if (apartment.OwnerId != ownerId && !isAdmin)
                throw new UnauthorizedAccessException("Access denied.");

            apartment.Update(dto);

            await _context.SaveChangesAsync();

            return _mapper.Map<ApartmentDto>(apartment);
        }

        public async Task<bool> DeleteAsync(int id, int ownerId)
        {
            var apartment = await _context.Apartments.FindAsync(id);

            if (apartment == null)
                return false;

            if (apartment.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You cannot delete this apartment.");

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ApartmentDto>> GetNearbyAsync(double userLat, double userLng, double maxDistanceKm, CancellationToken ct = default)
        {
            var maxMeters = maxDistanceKm * 1000;

            // إنشاء النقطة مع SRID 4326
            var userPoint = _geoFactory.CreatePoint(new Coordinate(userLng, userLat));
                userPoint.SRID = 4326;

            var latDelta = maxDistanceKm / 111.0;
            var lonDelta = maxDistanceKm / (111.0 * Math.Cos(userLat * Math.PI / 180));

            var minLat = userLat - latDelta;
            var maxLat = userLat + latDelta;
            var minLon = userLng - lonDelta;
            var maxLon = userLng + lonDelta;

            var apartments = await _context.Apartments
                .AsNoTracking()
                .Where(a => a.Location != null &&
                            a.Location.Location.Y >= minLat &&
                            a.Location.Location.Y <= maxLat &&
                            a.Location.Location.X >= minLon &&
                            a.Location.Location.X <= maxLon &&
                            a.Location.Location.IsWithinDistance(userPoint, maxMeters))
                .Select(a => new ApartmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    PricePerDay = a.PricePerDay!.Value,
                    PricePerHour = a.PricePerHour!.Value,
                    MediaUrls = a.MediaFiles!.Select(m => m.FileUrl).ToList(),
                    Latitude = a.Location!.Location.Y,
                    Longitude = a.Location.Location.X,
                    City = a.Location.City,
                    Country = a.Location.Country,
                    Area = a.Area,
                    FloorNumber = a.FloorNumber,
                    NumberOfBedRooms = a.NumberOfBedRooms,
                    NumberOfBalconies = a.NumberOfBalconies,
                    NumberOfLivingRooms = a.NumberOfLivingRooms,
                    NumberOfReceptionRooms = a.NumberOfReceptionRooms,
                    NumberOfBathrooms = a.NumberOfBathrooms,
                    IsChatEnabled = a.IsChatEnabled,
                    Street = a.Location.Street,
                    ApartmentNumber = a.Location.ApartmentNumber,
                    AverageRating = a.Reviews!.Any() ? a.Reviews!.Average(r => r.Rating) : 0,
                    CreatedAt = a.CreatedAt,
                    Finishing = a.Finishing,
                    Type = a.Type,
                    Description = a.Description,
                    View = a.View,
                    OwnerName = a.Owner!.FullName,
                    OwnerId = a.OwnerId,
                    Orientation = a.Orientation,
                    District = a.Location.District,
                    BuildingNumber = a.Location.BuildingNumber,
                    HasElevator = a.HasElevator,
                    HasParking = a.HasParking,
                    HasPool = a.HasPool,
                    TotalReviews = a.Reviews!.Count(),
                    IsAvailable = a.IsAvailable
                })
                .ToListAsync(ct);

            return apartments;
        }

        /*private double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
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
        }*/

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        
    }
}

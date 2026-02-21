using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using LaylaApi.Services.LocationFromIPService.Interfaces;
using LaylaApi.Services.DynamicApartmentSearchService;
using LaylaApi.Models.DtosModels.ExternalServicesDtos;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ApartmentService : IApartmentService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;
        private static readonly GeometryFactory _geoFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        private readonly ILocationFromIPExternalService _locationFromIP;
        private readonly IApartmentSearchService _apartmentSearchService;
        public ApartmentService(LaylaContext context, IMapper mapper, ILocationFromIPExternalService location, IApartmentSearchService apartmentSearchService)
        {
            _context = context;
            _mapper = mapper;
            _locationFromIP = location;
            _apartmentSearchService = apartmentSearchService;
        }

        public async Task<PagedResult<ApartmentDto>> GetTopBookedApartmentsAsync(CancellationToken ct = default, int pageNumber = 1, int pageSize = 10)
        {

            var items = await _context.Bookings
                .GroupBy(b => b.ApartmentId)  // Group by ID فقط
                .Select(g => new
                {
                    ApartmentId = g.Key,
                    TotalBookings = g.Count()
                })
                .OrderByDescending(x => x.TotalBookings)
                .Take(pageSize)
                .Join(_context.Apartments
                    .Include(a => a.Location)
                    .Include(a => a.MediaFiles)
                    .Include(a => a.Owner)
                    .Include(a => a.Reviews),
                    booking => booking.ApartmentId,
                    apartment => apartment.Id,
                    (booking, apartment) => new ApartmentDto
                    {
                        Id = apartment.Id,
                        Title = apartment.Title,
                        TotalBookings = booking.TotalBookings,
                        PricePerDay = apartment.PricePerDay!.Value,
                        PricePerHour = apartment.PricePerHour!.Value,
                        MediaUrls = apartment.MediaFiles!.Select(m => m.FileUrl).ToList(),
                        Latitude = apartment.Location!.Location.Y,
                        Longitude = apartment.Location.Location.X,
                        City = apartment.Location.City,
                        Country = apartment.Location.Country,
                        Area = apartment.Area,
                        FloorNumber = apartment.FloorNumber,
                        NumberOfBedRooms = apartment.NumberOfBedRooms,
                        NumberOfBalconies = apartment.NumberOfBalconies,
                        NumberOfLivingRooms = apartment.NumberOfLivingRooms,
                        NumberOfReceptionRooms = apartment.NumberOfReceptionRooms,
                        NumberOfBathrooms = apartment.NumberOfBathrooms,
                        IsChatEnabled = apartment.IsChatEnabled,
                        Street = apartment.Location.Street,
                        ApartmentNumber = apartment.Location.ApartmentNumber,
                        AverageRating = apartment.Reviews!.Any() ? apartment.Reviews!.Average(r => r.Rating) : 0,
                        CreatedAt = apartment.CreatedAt,
                        Finishing = apartment.Finishing,
                        Type = apartment.Type,
                        Description = apartment.Description,
                        View = apartment.View,
                        OwnerName = apartment.Owner!.FullName,
                        OwnerId = apartment.OwnerId,
                        Orientation = apartment.Orientation,
                        District = apartment.Location.District,
                        BuildingNumber = apartment.Location.BuildingNumber,
                        HasElevator = apartment.HasElevator,
                        HasParking = apartment.HasParking,
                        HasPool = apartment.HasPool,
                        TotalReviews = apartment.Reviews!.Count(),
                        IsAvailable = apartment.IsAvailable
                })
                 .OrderByDescending(x => x.TotalBookings)
                 .Take(pageSize)
                 .ToListAsync(ct);

            return new PagedResult<ApartmentDto>
            {
                Items = items,
                TotalCount = items.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ApartmentDto>> GetTopRatedApartmentsAsync(CancellationToken ct = default, int pageNumber = 1, int pageSize = 10)
        {
            var query = from review in _context.Reviews
                        group review by review.ApartmentId into g
                        select new
                        {
                            ApartmentId = g.Key,
                            AverageRating = g.Average(r => r.Rating),
                            TotalReviews = g.Count()
                        };

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.TotalReviews)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Join(_context.Apartments
                    .Include(a => a.Location)
                    .Include(a => a.MediaFiles)
                    .Include(a => a.Owner),
                    stats => stats.ApartmentId,
                    apartment => apartment.Id,
                    (stats, apartment) => new ApartmentDto
                    {
                        Id = apartment.Id,
                        AverageRating = stats.AverageRating,
                        TotalReviews = stats.TotalReviews,
                        Title = apartment!.Title,
                        PricePerDay = apartment.PricePerDay!.Value,
                        PricePerHour = apartment.PricePerHour!.Value,
                        MediaUrls = apartment.MediaFiles!.Select(m => m.FileUrl).ToList(),
                        Latitude = apartment.Location!.Location.Y,
                        Longitude = apartment.Location.Location.X,
                        City = apartment.Location.City,
                        Country = apartment.Location.Country,
                        Area = apartment.Area,
                        FloorNumber = apartment.FloorNumber,
                        NumberOfBedRooms = apartment.NumberOfBedRooms,
                        NumberOfBalconies = apartment.NumberOfBalconies,
                        NumberOfLivingRooms = apartment.NumberOfLivingRooms,
                        NumberOfReceptionRooms = apartment.NumberOfReceptionRooms,
                        NumberOfBathrooms = apartment.NumberOfBathrooms,
                        IsChatEnabled = apartment.IsChatEnabled,
                        Street = apartment.Location.Street,
                        ApartmentNumber = apartment.Location.ApartmentNumber,
                        CreatedAt = apartment.CreatedAt,
                        Finishing = apartment.Finishing,
                        Type = apartment.Type,
                        Description = apartment.Description,
                        View = apartment.View,
                        OwnerName = apartment.Owner!.FullName,
                        OwnerId = apartment.OwnerId,
                        Orientation = apartment.Orientation,
                        District = apartment.Location.District,
                        BuildingNumber = apartment.Location.BuildingNumber,
                        HasElevator = apartment.HasElevator,
                        HasParking = apartment.HasParking,
                        HasPool = apartment.HasPool,
                        IsAvailable = apartment.IsAvailable
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.TotalReviews)
                .Take(pageSize)
                .ToListAsync(ct);
            return new PagedResult<ApartmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PagedResult<ApartmentDto>> GetAllAsync(string userIp, CancellationToken ct)
        {
            // 1️⃣ محاولة البحث الجغرافي
            var location = await _locationFromIP.GetAsync(userIp, ct);

            if (IsValidLocation(location))
            {
                var geoResult = await SearchByLocationAsync(location!, ct);

                if (geoResult.Items.Any())
                    return geoResult;
            }

            // 2️⃣ الأكثر حجزًا
            var topBooked = await GetTopBookedApartmentsAsync(ct);

            if (topBooked.Items.Any())
                return topBooked;

            // 3️⃣ الأعلى تقييمًا
            var topRated = await GetTopRatedApartmentsAsync(ct);

            if (topRated.Items.Any())
                return topRated;

            // 4️⃣ fallback: المتاح فقط
            return await SearchAvailableAsync(ct);

        }

        public async Task<ApartmentDto> GetByIdAsync(int id, CancellationToken ct)
        {
            var apartment = await _context.Apartments
                .AsNoTracking()
                .Include(a => a.Owner)
                .Include(a => a.MediaFiles)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.Id == id, ct);

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
        public async Task <IEnumerable<ApartmentDto>> GetByOwnerIdAsync(int id, CancellationToken ct)
        {
            var apartments = await _context.Apartments
                  .AsNoTracking()
                  .Include(a => a.MediaFiles)
                  .Include(a => a.Reviews)
                  .Where(a => a.OwnerId == id)
                  .ToListAsync(ct);

            return _mapper.Map<IEnumerable<ApartmentDto>>(apartments);
        }

        public async Task<IEnumerable<ApartmentDto>> SearchAsync(string keyword, CancellationToken ct)
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
                .ToListAsync(ct);

            return _mapper.Map<IEnumerable<ApartmentDto>>(apartments);
        }

        public async Task<ApartmentDto> AddAsync(CreateApartmentDto dto, int userId, CancellationToken ct)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var ownerExists = await _context.Users.AnyAsync(u => u.Id == userId, ct);
            if (!ownerExists)
                throw new KeyNotFoundException("User not found");

            var apartment = Apartment.Create(dto, userId);

            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            return _mapper.Map<ApartmentDto>(apartment);
        }

        public async Task<ApartmentDto?> UpdateAsync(int id, CreateApartmentDto dto, int ownerId, bool isAdmin, CancellationToken ct)
        {
            var apartment = await _context.Apartments.FindAsync(id, ct)?? 
                throw new KeyNotFoundException("Apartment not found.");

            if (apartment.OwnerId != ownerId && !isAdmin)
                throw new UnauthorizedAccessException("Access denied.");

            apartment.Update(dto);

            await _context.SaveChangesAsync();

            return _mapper.Map<ApartmentDto>(apartment);
        }

        public async Task<bool> DeleteAsync(int id, int ownerId, CancellationToken ct)
        {
            var apartment = await _context.Apartments.FindAsync(id, ct);

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
                    AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : 0,
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
                    TotalReviews = a.Reviews.Count(),
                    IsAvailable = a.IsAvailable,
                    TotalBookings = a.Bookings.Count(),
                })
                .ToListAsync(ct);

            return apartments;
        }
        private static bool IsValidLocation(IpApiResponseDto? loc)
        {
            if (loc == null)
                return false;

            return loc.Lat is >= -90 and <= 90
                && loc.Lon is >= -180 and <= 180;
        }
        private Task<PagedResult<ApartmentDto>> SearchByLocationAsync(IpApiResponseDto loc, CancellationToken ct, double? minDistance = 1, double? maxDistance = 100)
        {
            var request = new ApartmentSearchRequestDto
            {
                City = loc.City,
                Country = loc.Country,
                UserLatitude = loc.Lat,
                UserLongitude = loc.Lon,
                MinDistance = minDistance,
                MaxDistance = maxDistance,
                IsAvailable = true
            };

            return _apartmentSearchService.SearchAsync(request, ct);
        }

        private Task<PagedResult<ApartmentDto>> SearchAvailableAsync(CancellationToken ct)
        {
            var request = new ApartmentSearchRequestDto
            {
                IsAvailable = true
            };

            return _apartmentSearchService.SearchAsync(request, ct);
        }

    }
}

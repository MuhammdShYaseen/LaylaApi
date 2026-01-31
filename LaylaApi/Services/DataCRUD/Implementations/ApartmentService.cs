using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ApartmentService : IApartmentService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;
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
            var apartment = await _context.Apartments.FindAsync(id)
        ?? throw new KeyNotFoundException("Apartment not found.");

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

        public async Task<IEnumerable<ApartmentDto>> GetNearbyAsync(double userLat, double userLng, double maxDistanceKm)
        {
            var apartments = await _context.Apartments
                .AsNoTracking()
                .Include(a => a.MediaFiles)
                .ToListAsync();

            var result = apartments
                .Where(a => CalculateDistanceKm(userLat, userLng, a.Location!.Location.Latitude, a.Location.Location.Longitude) <= maxDistanceKm)
                .ToList();

            return _mapper.Map<IEnumerable<ApartmentDto>>(result);
        }

        private double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
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

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        
    }
}

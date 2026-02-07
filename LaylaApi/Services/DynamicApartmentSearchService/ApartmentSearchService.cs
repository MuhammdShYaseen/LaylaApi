using AutoMapper;
using LaylaApi.DataRepository;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DynamicApartmentSearchService.BuilderServices;
using Microsoft.EntityFrameworkCore;
using System;

namespace LaylaApi.Services.DynamicApartmentSearchService
{
    public class ApartmentSearchService : IApartmentSearchService
    {
        private readonly IRepository<Apartment> _db;
        public ApartmentSearchService(IRepository<Apartment> db)
        {
            _db = db;
        }

        public async Task<PagedResult<ApartmentDto>> SearchAsync(ApartmentSearchRequestDto request, CancellationToken ct)
        {
            var predicate = ApartmentFilterBuilder.Build(request);
            request.PageSize = Math.Clamp(request.PageSize, 1, 50);
            request.PageNumber = Math.Max(request.PageNumber, 1);
            var query = _db.Query()
                .AsNoTracking()
                .Where(predicate);

            // Count
            var totalCount = await query.CountAsync(ct);

            // Sorting
            query = query.ApplySorting(
                request.SortBy,
                request.SortDirection);

            // Pagination
            var skip = (request.PageNumber - 1) * request.PageSize;

            // Projection
            var items = await query
                .Skip(skip)
                .Take(request.PageSize)
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

            return new PagedResult<ApartmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}


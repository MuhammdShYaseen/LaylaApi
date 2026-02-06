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
        private readonly IMapper _mapper;
        public ApartmentSearchService(IRepository<Apartment> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagedResult<ApartmentDto>> SearchAsync(
            ApartmentSearchRequestDto request,
            CancellationToken ct)
        {
            var predicate =
                ApartmentFilterBuilder.Build(request);

            var baseQuery = _db.Query()
                .AsNoTracking()
                .Where(predicate);

            var totalCount =
                await baseQuery.CountAsync(ct);

            var sorted =
                baseQuery.ApplySorting(
                    request.SortBy,
                    request.SortDirection);

            var skip =
                (request.PageNumber - 1) * request.PageSize;

            var data = await sorted
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync(ct);

            var dataDto = _mapper.Map<IEnumerable<ApartmentDto>>(data);
            return new PagedResult<ApartmentDto>
            {
                Items = dataDto.ToList(),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}


using LaylaApi.Models.DtosModels.MainDtos;

namespace LaylaApi.Services.DynamicApartmentSearchService
{
    public interface IApartmentSearchService
    {
        Task<PagedResult<ApartmentDto>> SearchAsync(ApartmentSearchRequestDto request, CancellationToken ct);
    }
}

using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using System.Linq.Expressions;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public interface IApartmentFilterBuilder
    {
        IQueryable<Apartment> Build(IQueryable<Apartment> query, ApartmentSearchRequestDto request);
    }
}

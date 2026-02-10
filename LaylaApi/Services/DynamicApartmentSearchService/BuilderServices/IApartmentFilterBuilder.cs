using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using System.Linq.Expressions;

namespace LaylaApi.Services.DynamicApartmentSearchService.BuilderServices
{
    public interface IApartmentFilterBuilder
    {
        Expression<Func<Apartment, bool>> Build(ApartmentSearchRequestDto request);
    }
}

using LaylaApi.Attributes;
using LaylaApi.Models.MainModels;
using System.Linq.Expressions;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ApartmentSortMap
    {
        [Sortable]
        public static Expression<Func<Apartment, object>> PricePerDay => x => x.PricePerDay!;

        [Sortable]
        public static Expression<Func<Apartment, object>> Area => x => x.Area;

        [Sortable]
        public static Expression<Func<Apartment, object>> CreatedAt => x => x.CreatedAt;

        [Sortable]
        public static Expression<Func<Apartment, object>> City => x => x.Location!.City;
    }
}

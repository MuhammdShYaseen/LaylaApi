using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Models.MainModels
{
    public class Country : Entity
    {
        // ISO 3166-1 Alpha-2 Code
        public string Code { get; set; } = null!;

        // Country name (normalized)
        public string Name { get; set; } = null!;

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}

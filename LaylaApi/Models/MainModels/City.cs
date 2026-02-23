using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Models.MainModels
{
    public class City : Entity
    {
        public string Name { get; private set; } = null!;

        public int CountryId { get; private set; }
        public Country Country { get; private set; } = null!;

        public ICollection<Apartment> Apartments { get; private set; } = new List<Apartment>();
    }
}

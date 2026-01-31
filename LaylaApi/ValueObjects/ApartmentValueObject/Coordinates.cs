using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Coordinates : ValueObject
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        private Coordinates() { } // EF Core
        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }

        public override string ToString()
            => $"{Latitude:0.######}, {Longitude:0.######}";
    }
}

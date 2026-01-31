using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Coordinates : ValueObject
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        private Coordinates() { } // EF Core
        public static Coordinates Create(double latitude, double longitude)
        {
            Validate(latitude, longitude);
            return new Coordinates
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        private static void Validate(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new BadHttpRequestException("Latitude must be between 90 and -90" + nameof(Latitude));

            if (longitude < -180 || longitude > 180)
                throw new BadHttpRequestException("Longitude must between 180 and -180" + nameof(Longitude));
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

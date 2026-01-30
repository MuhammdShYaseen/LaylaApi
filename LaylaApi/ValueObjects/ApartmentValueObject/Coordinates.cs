using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Coordinates : ValueObject
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;

            Validate();
        }

        private void Validate()
        {
            if (Latitude < -90 || Latitude > 90)
                throw new ArgumentException("خط العرض يجب أن يكون بين -90 و 90 درجة", nameof(Latitude));

            if (Longitude < -180 || Longitude > 180)
                throw new ArgumentException("خط الطول يجب أن يكون بين -180 و 180 درجة", nameof(Longitude));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // مقارنة بدقة 6 منازل عشرية (حوالي 10 سم دقة)
            yield return Math.Round(Latitude, 6);
            yield return Math.Round(Longitude, 6);
        }

        public override string ToString()
            => $"{Latitude:0.######}, {Longitude:0.######}";

        // طريقة لإنشاء إحداثيات جديدة
        public Coordinates With(double? latitude = null, double? longitude = null)
        {
            return new Coordinates(
                latitude ?? Latitude,
                longitude ?? Longitude
            );
        }
    }
}

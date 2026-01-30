using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class GeoLocation : ValueObject
    {
        
        
        public string Country { get; }
        public string City { get; }
        public string District { get; }
        public string Street { get; }
        public string BuildingNumber { get; }
        public string ApartmentNumber { get; }
        
        public Coordinates Location { get; } // مطلوبة - غير قابلة للقيمة الفارغة

        public GeoLocation(string street, string buildingNumber, string apartmentNumber, string city, string district, Coordinates location, string country) // إحداثيات مطلوبة
        {
            Street = street;
            BuildingNumber = buildingNumber;
            ApartmentNumber = apartmentNumber;
            City = city;
            District = district;
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Country = country;
            Validate();
        }

        public static GeoLocation Create(string  street, string buildingNumber, string  apartmentNumber,  string  city, string  district, Coordinates location , string country)
        {
            return new GeoLocation(street, buildingNumber, apartmentNumber, city, district, location, country);
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Street))
                throw new BadHttpRequestException("the name of street is required");

            if (string.IsNullOrWhiteSpace(BuildingNumber))
                throw new BadHttpRequestException("the number of building is required");

            if (string.IsNullOrWhiteSpace(ApartmentNumber))
                throw new BadHttpRequestException("Apartment number required");

            if (string.IsNullOrWhiteSpace(City))
                throw new BadHttpRequestException("City name is required");

            if (string.IsNullOrWhiteSpace(District))
                throw new BadHttpRequestException("District is required");

            if (string.IsNullOrWhiteSpace(Country))
                throw new BadHttpRequestException("Country is required");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // كل شيء يدخل في المقارنة بما في ذلك الإحداثيات
            yield return Street.ToUpperInvariant();
            yield return BuildingNumber.ToUpperInvariant();
            yield return ApartmentNumber.ToUpperInvariant();
            yield return City.ToUpperInvariant();
            yield return Country.ToUpperInvariant();
            yield return District.ToUpperInvariant();
            yield return Location; // الإحداثيات جزء من المساواة
        }

        public string GetFormattedAddress(bool includeCoordinates = true)
        {
            var parts = new List<string>
            {
                $"Apartment {ApartmentNumber}",
                $"Building {BuildingNumber}",
                $"Street {Street}",
                $"District {District}",
                City,
                Country
            };

            if (includeCoordinates)
            {
                parts.Add($"📍 {Location}");
            }

            return string.Join("، ", parts);
        }

        public string GetBasicAddress() => $"شقة {ApartmentNumber}، مبنى {BuildingNumber}، {Street}، {District}، {City} ، {Country}";

        public override string ToString() => GetFormattedAddress();


        // طريقة للتحقق إذا كان العنوان في منطقة معينة (بدون حسابات معقدة)
        public bool IsSameArea(GeoLocation other)
        {
            if (other == null) return false;

            // تحقق إذا كان نفس الحي والمدينة
            return string.Equals(District, other.District, StringComparison.OrdinalIgnoreCase) && string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase);
        }
    }

}

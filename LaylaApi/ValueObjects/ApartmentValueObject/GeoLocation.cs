using LaylaApi.DomainEvents.Domain.Common;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class GeoLocation : ValueObject
    {
        private GeoLocation() { } // EF Core

        public string Street { get; private set; } = null!;
        public string BuildingNumber { get; private set; } = null!;
        public string ApartmentNumber { get; private set; } = null!;
        public string City { get; private set; } = null!;
        public string District { get; private set; } = null!;
        public string Country { get; private set; } = null!;
        public Point Location { get; private set; } = null!;

        public static GeoLocation Create(string street, string buildingNumber, string apartmentNumber,
                                         string city, string district, Coordinates coordinates, string country)
        {
            Validate(street, buildingNumber, apartmentNumber,
                     city, district, coordinates, country);

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var point = geometryFactory.CreatePoint(new Coordinate(coordinates.Longitude, coordinates.Latitude ));
            point.SRID = 4326;
            return new GeoLocation
            {
                Street = street,
                BuildingNumber = buildingNumber,
                ApartmentNumber = apartmentNumber,
                City = city,
                District = district,
                Country = country,
                Location = point ?? throw new ArgumentNullException(nameof(point))
            };
            
        }

       

        private static void Validate(string street, string buildingNumber, string apartmentNumber,
                                     string city, string district, Coordinates location, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new BadHttpRequestException("the name of street is required");

            if (string.IsNullOrWhiteSpace(buildingNumber))
                throw new BadHttpRequestException("the number of building is required");

            if (string.IsNullOrWhiteSpace(apartmentNumber))
                throw new BadHttpRequestException("Apartment number required");

            if (string.IsNullOrWhiteSpace(city))
                throw new BadHttpRequestException("City name is required");

            if (string.IsNullOrWhiteSpace(district))
                throw new BadHttpRequestException("District is required");

            if (string.IsNullOrWhiteSpace(country))
                throw new BadHttpRequestException("Country is required");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street.ToUpperInvariant();
            yield return BuildingNumber.ToUpperInvariant();
            yield return ApartmentNumber.ToUpperInvariant();
            yield return City.ToUpperInvariant();
            yield return District.ToUpperInvariant();
            yield return Country.ToUpperInvariant();
            yield return Location;
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

        public string GetBasicAddress() => $"apartment {ApartmentNumber}، building {BuildingNumber}، {Street}، {District}، {City} ، {Country}";

        public override string ToString() => GetFormattedAddress();


        // طريقة للتحقق إذا كان العنوان في منطقة معينة (بدون حسابات معقدة)
       
    }

}

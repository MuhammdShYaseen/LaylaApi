using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Pricing : ValueObject
    {
        public Money PricePerHour { get; private set; } = null!;
        public Money PricePerDay { get; private set; } = null!;

        private Pricing()
        {
            
        }

        public static Pricing Create(Money pricePerHour, Money pricePerDay)
        {
            Validate(pricePerHour, pricePerDay);
            return new Pricing
            {
                PricePerHour = pricePerHour,
                PricePerDay = pricePerDay
            };
        }

        private static void Validate(Money pricePerHour, Money pricePerDay)
        {
            if (pricePerHour is null)
                throw new ArgumentNullException(nameof(pricePerHour));

            if (pricePerDay is null)
                throw new ArgumentNullException(nameof(pricePerDay));

            if (pricePerHour.Value <= 0)
                throw new ArgumentException("Hourly price must be greater than zero.");

            if (pricePerDay.Value <= 0)
                throw new ArgumentException("Daily price must be greater than zero.");

            if (pricePerDay.Value < pricePerHour.Value)
                throw new ArgumentException("Daily price cannot be less than hourly price.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PricePerHour;
            yield return PricePerDay;
        }
    }
}

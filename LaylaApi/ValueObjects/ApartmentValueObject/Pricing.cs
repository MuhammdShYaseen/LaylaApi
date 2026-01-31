using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Pricing : ValueObject
    {
        public Money PricePerHour { get; private set; }
        public Money PricePerDay { get; private set; }

        private Pricing(Money pricePerHour, Money pricePerDay)
        {
            PricePerHour = pricePerHour;
            PricePerDay = pricePerDay;
        }

        public static Pricing Create(Money pricePerHour, Money pricePerDay)
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

            return new Pricing(pricePerHour, pricePerDay);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PricePerHour;
            yield return PricePerDay;
        }
    }
}

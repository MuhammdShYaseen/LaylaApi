using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public sealed class Money : ValueObject
    {
        public decimal Value { get; private set; }

        private Money(decimal value)
        {
            if (value <= 0)
                throw new BadHttpRequestException("Price must be greater than zero.");

            Value = value;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static Money Create(decimal value)
            => new Money(value);
        // Comparisons
        public int CompareTo(Money? other)
            => Value.CompareTo(other!.Value);

        public static bool operator >=(Money a, Money b)
            => a.Value >= b.Value;

        public static bool operator <=(Money a, Money b)
            => a.Value <= b.Value;

        public static bool operator >(Money a, Money b)
            => a.Value > b.Value;

        public static bool operator <(Money a, Money b)
            => a.Value < b.Value;
    }
}

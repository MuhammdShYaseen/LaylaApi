using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Description : ValueObject
    {
        public string Value { get; private set; }

        private Description(string value)
        {
            Value = value;
        }

        public static Description? Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (value.Length > 1000)
                throw new ArgumentException("Description must not exceed 1000 characters.");

            return new Description(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

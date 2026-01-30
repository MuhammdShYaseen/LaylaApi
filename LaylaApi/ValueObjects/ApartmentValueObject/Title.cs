using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.ValueObjects.ApartmentValueObject
{
    public class Title : ValueObject
    {
        public string Value { get; }

        private Title(string value)
        {
            Value = value;
        }

        public static Title Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title is required.");

            value = value.Trim();

            if (value.Length > 200)
                throw new ArgumentException("Title must not exceed 200 characters.");

            if (value.Length < 3)
                throw new ArgumentException("Title is too short.");

            return new Title(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

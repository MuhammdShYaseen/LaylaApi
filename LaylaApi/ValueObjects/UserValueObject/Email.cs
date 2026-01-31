using LaylaApi.DomainEvents.Domain.Common;
using System.Text.RegularExpressions;

namespace LaylaApi.ValueObjects.UserValueObject
{
    public class Email : ValueObject
    {
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email is required.");

            value = value.Trim().ToLowerInvariant();

            if (value.Length > 320)
                throw new ArgumentException("Email is too long.");

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Invalid email format.");

            return new Email(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}

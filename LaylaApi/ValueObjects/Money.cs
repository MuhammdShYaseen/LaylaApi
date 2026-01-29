namespace LaylaApi.ValueObjects
{
    public sealed class Money
    {
        public decimal Value { get; }

        private Money(decimal value)
        {
            if (value <= 0)
                throw new BadHttpRequestException("Price must be greater than zero.");

            Value = value;
        }

        public static Money Create(decimal value)
            => new Money(value);
    }
}

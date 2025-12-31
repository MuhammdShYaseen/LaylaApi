namespace LaylaApi.DomainEvents.Domain.Events
{
    public class PaymentCompletedEvent : IEvent
    {
        public int PaymentId { get; }
        public int BookingId { get; }
        public decimal Amount { get; }

        public PaymentCompletedEvent(int paymentId, int bookingId, decimal amount)
        {
            PaymentId = paymentId;
            BookingId = bookingId;
            Amount = amount;
        }
    }
}
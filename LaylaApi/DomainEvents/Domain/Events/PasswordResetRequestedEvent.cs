namespace LaylaApi.DomainEvents.Domain.Events
{
    public class PasswordResetRequestedEvent : IEvent
    {
        public int UserId { get; }
        public string Email { get; }
        public string Token { get; }

        public PasswordResetRequestedEvent(int userId, string email, string token)
        {
            UserId = userId;
            Email = email;
            Token = token;
        }
    }
}

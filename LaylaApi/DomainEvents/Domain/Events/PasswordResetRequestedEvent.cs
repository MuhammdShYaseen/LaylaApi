using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class PasswordResetRequestedEvent : IEvent
    {
        public User User { get; }
        public string Token { get; }

        public PasswordResetRequestedEvent(User user, string token)
        {
             User = user;
             Token = token;
        }
    }
}

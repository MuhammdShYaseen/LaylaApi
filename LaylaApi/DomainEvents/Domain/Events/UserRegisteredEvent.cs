using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class UserRegisteredEvent : IEvent
    {
        public User User { get; }
        public UserRegisteredEvent(User user)
        {
            User = user;
        }
    }
}

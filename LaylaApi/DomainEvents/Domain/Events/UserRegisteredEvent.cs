using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class UserRegisteredEvent : IEvent
    {
        public User User { get; }
        public string Token { get; }
        public UserRegisteredEvent(User user, string token)
        {
            User = user;
            Token = token;
        }
    }
}

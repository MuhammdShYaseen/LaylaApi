using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class UserRegisteredEvent : IEvent
    {
        public int UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public UserRegisteredEvent(int userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }
}

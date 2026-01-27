using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class PasswordResetRequestedEvent : IEvent
    {
        public Guid UserGuid { get; }
        public string Token { get; }

        public PasswordResetRequestedEvent(Guid userGuid, string token)
        {
            UserGuid = userGuid;
            Token = token;
        }
    }
}

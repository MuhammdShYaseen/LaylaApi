namespace LaylaApi.DomainEvents.Domain.Events
{
    public class UserEmailChangedEvent : IEvent
    {
        public Guid UserGuid { get; }
        public string NewEmail {  get; }
        public string FullName { get; }
        public string Language { get; }
        public string EmailVerificationToken { get; }
        public DateTime? EmailVerificationTokenExpires { get; }
        public UserEmailChangedEvent(Guid userGuid, string newEmail, string fullName, string language, string emailVerificationToken, DateTime? emailVerificationTokenExpires)
        {
            UserGuid = userGuid;
            NewEmail = newEmail;
            FullName = fullName;
            Language = language;
            EmailVerificationToken = emailVerificationToken;
            EmailVerificationTokenExpires = emailVerificationTokenExpires;
        }
    }
}

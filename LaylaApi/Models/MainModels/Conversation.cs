using FirebaseAdmin.Messaging;
using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Models.MainModels
{
    public class Conversation : Entity
    {
        public int ApartmentId { get; private set; }
        public Apartment? Apartment { get; set; }
        public int OwnerId { get; private set; }
        public int UserId { get; private set; }
        public bool IsClosedByOwner { get; private set; }
        public ICollection<Message>? Messages { get; set; }

        public static Conversation Create(int apartmentId, int ownerId, int userId)
        {
            return new Conversation
            {
                ApartmentId = apartmentId,
                OwnerId = ownerId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsClosedByOwner = false
            };
        }

        public void CloseConversation()
        {
            IsClosedByOwner = true;
        }

        public void OpenConversation()
        {
            IsClosedByOwner = false;
        }
    }
}

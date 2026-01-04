using FirebaseAdmin.Messaging;
using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Models.MainModels
{
    public class Conversation 
    {
        public int Id { get; set; }  
        public int ApartmentId { get; set; }
        public Apartment? Apartment { get; set; }
        public int OwnerId { get; set; }
        public int UserId { get; set; }
        public bool IsClosedByOwner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message>? Messages { get; set; }

        public static Conversation Create(int apartmentId, int ownerId, int userId)
        {
            return new Conversation
            {
                ApartmentId = apartmentId,
                OwnerId = ownerId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsClosedByOwner = false
            };
        }
    }
}

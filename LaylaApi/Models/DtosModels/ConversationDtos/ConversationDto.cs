namespace LaylaApi.Models.DtosModels.ConversationDtos
{
    public class ConversationDto
    {
        public int Id { get; set; }

        // Apartment
        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; } = default!;
        public string? ApartmentImageUrl { get; set; }

        // Participants
        public int OwnerId { get; set; }
        public int UserId { get; set; }

        public string UserName { get; set; } = default!;
        public string OwnerName { get; set; } = default!;

        // State
        public bool IsClosedByOwner { get; set; }

        // Last Message Preview
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // UX
        public int UnreadCount { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
    }
}

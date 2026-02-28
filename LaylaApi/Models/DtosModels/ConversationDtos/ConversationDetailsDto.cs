using LaylaApi.Models.DtosModels.MessageDtos;

namespace LaylaApi.Models.DtosModels.ConversationDtos
{
    public class ConversationDetailsDto
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }

        public string ApartmentTitle { get; set; } = default!;

        public int OwnerId { get; set; }

        public int UserId { get; set; }

        public bool IsClosedByOwner { get; set; }

        public IReadOnlyList<MessageDto> Messages { get; set; } = [];

        public DateTime CreatedAt { get; set; }
    }
}

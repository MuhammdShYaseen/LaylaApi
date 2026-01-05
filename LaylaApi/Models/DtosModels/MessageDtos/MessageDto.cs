using static LaylaApi.Models.MainModels.Message;

namespace LaylaApi.Models.DtosModels.MessageDtos
{
    public sealed class MessageDto
    {
        public int Id { get; init; }

        public int ConversationId { get; init; }

        public int SenderId { get; init; }

        public MessageType Type { get; init; }

        public string? Content { get; init; }

        public string? VoiceUrl { get; init; }

        public int? VoiceDurationSeconds { get; init; }

        public DateTime SentAt { get; init; }
    }
}

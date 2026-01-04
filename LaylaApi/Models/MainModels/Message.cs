using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using Microsoft.VisualBasic;

namespace LaylaApi.Models.MainModels
{
    public class Message : Entity
    {
        public enum MessageType
        {
            Text = 1,
            Voice = 2
        }
        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }
        public int SenderId { get; set; }
        public MessageType Type { get; set; }
        public string? Content { get; set; }
        public string? VoiceFilePath { get; set; }
        public int? VoiceDurationSeconds { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
   
    public static Message Create(int conversationId, int senderId, MessageType messageType, string content, string voiceFilePath, int voiceDurationSeconds, Conversation conversation)
        {
            var receiverId = senderId == conversation.OwnerId? conversation.UserId : conversation.OwnerId;
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Type = messageType,
                Content = content,
                VoiceFilePath = voiceFilePath,
                VoiceDurationSeconds = voiceDurationSeconds
            };
            message.AddDomainEvent(new MessageSentDomainEvent(message, receiverId));
            return message;
        } 
    }
}

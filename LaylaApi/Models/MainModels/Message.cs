using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.Models.MainModels
{
    public class Message : Entity
    {
        public enum MessageType
        {
            Text = 1,
            Voice = 2
        }
        public int ConversationId { get; private set; }
        public Conversation? Conversation { get; set; }
        public int SenderId { get; private set; }
        public MessageType Type { get; private set; }
        public string? Content { get; private set; }
        public string? VoiceFilePath { get; private set; }
        public int? VoiceDurationSeconds { get; private set; }
   
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
            message.AddDomainEvent(new MessageSentDomainEvent(conversationId, senderId, receiverId, content, conversation.ApartmentId));
            return message;
        } 

        public void DeleteVoiceFilePath()
        {
            VoiceFilePath = null;
        }

        public void SetVoiceFilePath(string voiceFilePath)
        {
            VoiceFilePath = VoiceFilePath ?? string.Empty;
        }
    }
}

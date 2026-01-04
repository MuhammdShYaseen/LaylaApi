using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Exceptions;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.ChatServices.Interfaces;
using static LaylaApi.Models.MainModels.Message;

namespace LaylaApi.Services.ChatServices.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly LaylaContext _context;
        private readonly IVoiceStorageService _voiceStorage;
        public MessageService(LaylaContext context, IVoiceStorageService voiceStorage)
        {
            _context = context;
            _voiceStorage = voiceStorage;
        }
        public async Task<Message> SendTextAsync(int conversationId, int senderId, string content)
        {
            var conversation = await ValidateConversation(conversationId, senderId);
            
            var message = Message.Create(conversationId, senderId, MessageType.Text, content, "", 0, conversation);

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;

        }

        public async Task<Message> SendVoiceAsync(int conversationId, int senderId, IFormFile file, int duration)
        {
            var conversation = await ValidateConversation(conversationId, senderId);

            var message = Message.Create(conversationId, senderId, MessageType.Voice, "Voice Message", "", duration, conversation);

            _context.Messages.Add(message);

            await _context.SaveChangesAsync();

            message.VoiceFilePath = await _voiceStorage.SaveAsync(file, message.Id);

            await _context.SaveChangesAsync();

            return message;
        }

        private async Task<Conversation> ValidateConversation(int conversationId, int senderId)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId)?? 
                throw new KeyNotFoundException();

            if (conversation.IsClosedByOwner)
                throw new BadHttpRequestException("Chat was closed by owner");

            if (senderId != conversation.OwnerId && senderId != conversation.UserId)
                throw new UnauthorizedAccessException("Access denied");

            return conversation;
        }
    }
}

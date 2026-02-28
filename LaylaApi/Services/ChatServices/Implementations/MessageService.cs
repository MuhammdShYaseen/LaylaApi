using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Exceptions;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security;
using static LaylaApi.Models.MainModels.Message;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<Message> SendTextAsync(int conversationId, int senderId, string content, CancellationToken ct)
        {
            var conversation = await ValidateConversation(conversationId, senderId, ct);

            var message = Message.Create(conversationId, senderId, MessageType.Text, content, "", 0, conversation);

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;

        }

        public async Task<Message> SendVoiceAsync(int conversationId, int senderId, IFormFile file, int duration, CancellationToken ct)
        {
            var conversation = await ValidateConversation(conversationId, senderId, ct);

            var message = Message.Create(conversationId, senderId, MessageType.Voice, "Voice Message", "", duration, conversation);

            _context.Messages.Add(message);

            await _context.SaveChangesAsync();

            var voiceFilePath = await _voiceStorage.SaveAsync(file, message.Id);

            message.SetVoiceFilePath(voiceFilePath);

            await _context.SaveChangesAsync();

            return message;
        }

        private async Task<Conversation> ValidateConversation(int conversationId, int senderId, CancellationToken ct)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId, ct)?? 
                throw new KeyNotFoundException();

            if (conversation.IsClosedByOwner)
                throw new BadHttpRequestException("Chat was closed by owner");

            if (senderId != conversation.OwnerId && senderId != conversation.UserId)
                throw new UnauthorizedAccessException("Access denied");

            return conversation;
        }

        public async Task<bool> MarkAsReadAsync(int conversationId, int userId, CancellationToken ct)
        {
            var isParticipant = await _context.Conversations
                .AnyAsync(c => c.Id == conversationId && (c.UserId == userId || c.OwnerId == userId), ct);

            if (!isParticipant)
                throw new UnauthorizedAccessException();

            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId && m.ReceiverId == userId && !m.IsRead)
                .ToListAsync(ct);

             foreach (var msg in messages)
             {
                 msg.SetAsRead(userId);
            }

            var updated =  await _context.SaveChangesAsync();
            return updated > 0;
        }
    }
}

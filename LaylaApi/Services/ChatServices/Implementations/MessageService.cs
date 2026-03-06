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
            ValidateVoiceFile(file);
            var conversation = await ValidateConversation(conversationId, senderId, ct);

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var message = Message.Create(conversationId, senderId, MessageType.Voice, "Voice Message", "", duration, conversation);

                _context.Messages.Add(message);

                await _context.SaveChangesAsync();

                var voiceFilePath = await _voiceStorage.SaveAsync(file, message.Id);

                message.SetVoiceFilePath(voiceFilePath);

                await _context.SaveChangesAsync();

                return message;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
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

        private void ValidateVoiceFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadHttpRequestException("Empty file");

            const long maxSize = 2 * 1024 * 1024; // 2 MB

            if (file.Length > maxSize)
                throw new BadHttpRequestException("File too large");

            var allowedTypes = new[]
            {
                "audio/mpeg",
                "audio/wav",
                "audio/ogg"
            };

            if (!allowedTypes.Contains(file.ContentType))
                throw new BadHttpRequestException("Invalid audio format");
        }

        public async Task<bool> MarkAsReadAsync(int conversationId, int userId, CancellationToken ct)
        {
            var isParticipant = await _context.Conversations
                .AnyAsync(c => c.Id == conversationId && (c.UserId == userId || c.OwnerId == userId), ct);

            if (!isParticipant)
                throw new UnauthorizedAccessException();

            var updated = await _context.Messages
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.ReceiverId == userId &&
                    !m.IsRead)
                .ExecuteUpdateAsync(
                    setters => setters
                .SetProperty(m => m.IsRead, true), ct);

            return updated > 0;
        }
    }
}

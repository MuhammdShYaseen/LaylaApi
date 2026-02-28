using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.ConversationDtos;
using LaylaApi.Models.DtosModels.MessageDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.ChatServices.Implementations
{
    public class ConversationService : IConversationService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;
        public ConversationService(LaylaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CloseAsync(int conversationId, int ownerId, CancellationToken ct)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId, ct)?? 
                throw new KeyNotFoundException("conversation not found");

            if (conversation.OwnerId != ownerId)
                throw new UnauthorizedAccessException("you cannot close this chat");

            conversation.CloseConversation();
            await _context.SaveChangesAsync();
        }

        public async Task OpenAsync(int conversationId, int ownerId, CancellationToken ct)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId, ct) ??
                throw new KeyNotFoundException("conversation not found");

            if (conversation.OwnerId != ownerId)
                throw new UnauthorizedAccessException("you cannot Open this chat");

            conversation.OpenConversation();

            await _context.SaveChangesAsync();
        }

        public async Task<Conversation> GetOrCreateAsync(int apartmentId, int userId, CancellationToken ct)
        {
            var apartment = await _context.Apartments.FindAsync(apartmentId, ct)?? 
                throw new KeyNotFoundException("Apartment Not found");

            if (!apartment.IsChatEnabled)
                throw new BadHttpRequestException("Chat is not Enabled on this apartment");

                var conversation = await _context.Conversations.
                                   FirstOrDefaultAsync(x => x.ApartmentId == apartmentId && x.UserId == userId, ct);
           
            if (conversation != null)
                return conversation;

            conversation = Conversation.Create(apartmentId, apartment.OwnerId, userId);

            _context.Conversations.Add(conversation);

            await _context.SaveChangesAsync();

            return conversation;
        }

        public async Task<IReadOnlyList<ConversationDto>> GetUserConversationsAsync(int userId, CancellationToken ct)
        {
            return await _context.Conversations
                .AsNoTracking()

                 // Authorization at Query Level
                .Where(c => c.UserId == userId || c.OwnerId == userId)

                // Performance: Project in DB
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    ApartmentId = c.ApartmentId,
                    ApartmentTitle = c.Apartment.Title,
                    ApartmentImageUrl = c.Apartment.MediaFiles
                        .Where(m => m.IsPrimary)
                        .Select(m => m.FileUrl)
                        .FirstOrDefault(),
                    OwnerId = c.OwnerId,
                    UserId = c.UserId,
                    UserName = c.User.FullName,
                    OwnerName = c.Apartment.Owner.FullName,
                    IsClosedByOwner = c.IsClosedByOwner,
                    LastMessage = c.Messages!
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Content)
                        .FirstOrDefault(),
                    LastMessageAt = c.Messages!.Max(m => (DateTime?)m.CreatedAt),
                    UnreadCount = c.Messages!.Count(m => !m.IsRead && m.ReceiverId == userId),
                    CreatedAt = c.CreatedAt
                })

                // UX: newest first
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)

                .ToListAsync(ct);
        }

        public async Task<ConversationDetailsDto?> GetDetailsAsync(int conversationId, int userId, CancellationToken ct)
        {
            return await _context.Conversations
                .AsNoTracking()

                // Authorization
                 .Where(c => c.Id == conversationId && (c.UserId == userId || c.OwnerId == userId))

                .Select(c => new ConversationDetailsDto
                {
                    Id = c.Id,
                    ApartmentId = c.ApartmentId,
                    ApartmentTitle = c.Apartment.Title,
                    OwnerId = c.OwnerId,
                    UserId = c.UserId,
                    IsClosedByOwner = c.IsClosedByOwner,
                    CreatedAt = c.CreatedAt,
                    Messages = c.Messages!
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => new MessageDto
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            Content = m.Content,
                            Type = m.Type,
                            VoiceUrl = m.VoiceFilePath,
                            VoiceDurationSeconds = m.VoiceDurationSeconds,
                            IsRead = m.IsRead,
                            SentAt = m.CreatedAt
                        })
                        .ToList()
                })

                .FirstOrDefaultAsync(ct);
        }
    }
}

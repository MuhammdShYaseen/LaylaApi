using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.ChatServices.Implementations
{
    public class ConversationService : IConversationService
    {
        private readonly LaylaContext _context;
        public ConversationService(LaylaContext context)
        {
            _context = context;
        }
        public async Task CloseAsync(int conversationId, int ownerId)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId)?? 
                throw new KeyNotFoundException("conversation not found");

            if (conversation.OwnerId != ownerId)
                throw new UnauthorizedAccessException("you cannot close this chat");

            conversation.IsClosedByOwner = true;
            await _context.SaveChangesAsync();
        }

        public async Task<Conversation> GetOrCreateAsync(int apartmentId, int userId)
        {
            var apartment = await _context.Apartments.FindAsync(apartmentId)?? 
                throw new KeyNotFoundException("Apartment Not found");

            if (!apartment.IsChatEnabled)
                throw new BadHttpRequestException("Chat is not Enabled on this apartment");

                var conversation = await _context.Conversations.
                                   FirstOrDefaultAsync(x => x.ApartmentId == apartmentId && x.UserId == userId);
           
            if (conversation != null)
                return conversation;

            conversation = Conversation.Create(apartmentId, apartment.OwnerId, userId);

            _context.Conversations.Add(conversation);

            await _context.SaveChangesAsync();

            return conversation;
        }
    }
}

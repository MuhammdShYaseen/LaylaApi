using LaylaApi.DataAccess;
using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.ChatServices.Implementations
{
    public class ConversationReadService : IConversationReadService
    {
        private readonly LaylaContext _context;

        public ConversationReadService(LaylaContext context)
        {
            _context = context;
        }
        public Task<bool> IsParticipantAsync(int conversationId, int userId)
        {
            return _context.Conversations
             .AsNoTracking()
             .AnyAsync(c =>
                 c.Id == conversationId &&
                 (c.OwnerId == userId || c.UserId == userId));
        }
        public async Task<List<int>> GetUserConversationIdsAsync(int userId)
        {
            
             return await _context.Conversations
                    .AsNoTracking()
                    .Where(c =>
                        c.UserId == userId ||
                        c.OwnerId == userId)
                    .Select(c => c.Id)
                    .ToListAsync();
            
        }
    }
}

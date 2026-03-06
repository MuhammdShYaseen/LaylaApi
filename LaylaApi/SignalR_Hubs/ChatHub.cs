using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace LaylaApi.SignalR_Hubs
{
    [Authorize(Policy = "ConfirmedEmail")]
    public class ChatHub : Hub
    {
        private readonly IConversationReadService _conversationRead;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(IConversationReadService conversationRead, ILogger<ChatHub> logger)
        {
            _conversationRead = conversationRead;
            _logger = logger;
        }
        private int GetUserId()
        {
            var id = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (string.IsNullOrEmpty(id))
                throw new HubException("Unauthenticated");

            return int.Parse(id);
        }
        public async Task JoinConversation(int conversationId)
        {
            var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isParticipant = await _conversationRead.IsParticipantAsync(conversationId, userId);

            if (!isParticipant)
            {
                _logger.LogWarning("Unauthorized hub access: User {UserId} tried to join {ConversationId}", userId, conversationId);

                throw new HubException("Access denied");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();

            var conversations =
                await _conversationRead
                    .GetUserConversationIdsAsync(userId);

            foreach (var id in conversations)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    id.ToString());
            }

            await base.OnConnectedAsync();
        }

    }
}

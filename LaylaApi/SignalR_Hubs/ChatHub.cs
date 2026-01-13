using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace LaylaApi.SignalR_Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationReadService _conversationRead;

        public ChatHub(IConversationReadService conversationRead)
        {
            _conversationRead = conversationRead;
        }
        public async Task JoinConversation(int conversationId)
        {
            var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isParticipant = await _conversationRead.IsParticipantAsync(conversationId, userId);

            if (!isParticipant)
                throw new HubException("Access denied");

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

    }
}

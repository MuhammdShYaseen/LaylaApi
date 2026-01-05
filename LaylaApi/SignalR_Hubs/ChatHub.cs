using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace LaylaApi.SignalR_Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,
            conversationId.ToString());
        }

    }
}

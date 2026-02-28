using AutoMapper;
using LaylaApi.Models.DtosModels.MessageDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.ChatServices.Interfaces;
using LaylaApi.SignalR_Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/messages")]
    [Authorize(Policy = "ConfirmedEmail")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IMapper _mapper;
        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hub, IConversationService conversationService, IMapper mapper)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _mapper = mapper;
            _hub = hub;
        }

        private int GetUserID()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return userId;
        }

        [HttpPost("text")]
        public async Task<IActionResult> SendText(SendTextDto dto, CancellationToken ct)
        {
            var userId = GetUserID();

            var conversation =await _conversationService.GetOrCreateAsync(dto.ApartmentId, userId, ct);

            var message = await _messageService.SendTextAsync(conversation.Id, userId, dto.Content, ct);

            await _hub.Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveMessage", message, ct);

            return Ok(_mapper.Map<MessageDto>(message));
        }

        [HttpPost("voice")]
        public async Task<IActionResult> SendVoice([FromForm] SendVoiceDto dto, CancellationToken ct)
        {
            var userId = GetUserID();

            var conversation = await _conversationService.GetOrCreateAsync(dto.ApartmentId, userId, ct);

            var message = await _messageService.SendVoiceAsync(conversation.Id, userId, dto.AudioFile, dto.DurationSeconds, ct);

            await _hub.Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveMessage", message, ct);

            return Ok(_mapper.Map<MessageDto>(message));
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkAsRead(int conversationId, CancellationToken ct)
        {
            var userId = GetUserID();

            var updated = await _messageService.MarkAsReadAsync(conversationId, userId, ct);

            if (!updated)
                return NoContent(); // already read
            var messageReadEvent = new MessagesReadEvent(conversationId, userId, DateTime.UtcNow);

            await _hub.Clients.Group(conversationId.ToString()).SendAsync("MessagesRead", messageReadEvent, ct);

            return Ok();
        }

    }
}

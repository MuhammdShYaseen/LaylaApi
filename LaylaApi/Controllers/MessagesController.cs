using AutoMapper;
using LaylaApi.Models.DtosModels.MessageDtos;
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
        public async Task<IActionResult> SendText(SendTextDto dto)
        {
            var userId = GetUserID();

            var conversation =await _conversationService.GetOrCreateAsync(dto.ApartmentId, userId);

            var message = await _messageService.SendTextAsync(conversation.Id, userId, dto.Content);

            await _hub.Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveMessage", message);

            return Ok(_mapper.Map<MessageDto>(message));
        }

        [HttpPost("voice")]
        public async Task<IActionResult> SendVoice([FromForm] SendVoiceDto dto)
        {
            var userId = GetUserID();

            var conversation =await _conversationService.GetOrCreateAsync(dto.ApartmentId, userId);

            var message = await _messageService.SendVoiceAsync(conversation.Id, userId, dto.AudioFile, dto.DurationSeconds);

            await _hub.Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveMessage", message);

            return Ok(_mapper.Map<MessageDto>(message));
        }

    }
}

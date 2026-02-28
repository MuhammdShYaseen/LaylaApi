using LaylaApi.Models.DtosModels.ConversationDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize(Policy = "ConfirmedEmail")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationQuery)
        {
            _conversationService = conversationQuery;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 1️⃣ Inbox
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetMyConversations(CancellationToken ct)
        {
            var userId = GetUserId();

            var result = await _conversationService.GetUserConversationsAsync(userId, ct);

            return Ok(result);
        }

        // 2️⃣ Conversation Details
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ConversationDetailsDto>> GetConversation(int id, CancellationToken ct)
        {
            var userId = GetUserId();

            var result = await _conversationService.GetDetailsAsync(id, userId, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // 3️⃣ Close Conversation
        [HttpPost("{id:int}/close")]
        public async Task<IActionResult> CloseConversation(int id, CancellationToken ct)
        {
            var userId = GetUserId();

            await _conversationService.CloseAsync(id, userId, ct);

            return NoContent();
        }

        [HttpPost("{id:int}/Open")]
        public async Task<IActionResult> OpenConversation(int id, CancellationToken ct)
        {
            var userId = GetUserId();

            await _conversationService.OpenAsync(id, userId, ct);

            return NoContent();
        }

    }
}

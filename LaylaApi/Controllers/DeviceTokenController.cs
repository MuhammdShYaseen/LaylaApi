using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/device-tokens")]
    public class DeviceTokensController : ControllerBase
    {
        private readonly IDeviceTokenService _service;

        public DeviceTokensController(IDeviceTokenService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(id, out var userId))
                throw new UnauthorizedAccessException("Invalid token");

            return userId;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId, CancellationToken ct)
        {
            var tokens = await _service.GetByUserIdAsync(userId, ct);

            return Ok(ApiResponse<IEnumerable<DeviceTokenDto>>.Ok(tokens));
        }

        [HttpPut]
        public async Task<IActionResult> Upsert([FromBody] DeviceTokenUpsertDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<DeviceToken>.Fail("Invalid payload"));

            var token = await _service.UpsertAsync(dto, GetUserId(), ct);

            return Ok(ApiResponse<DeviceTokenDto>.Ok(token));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(id, ct);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

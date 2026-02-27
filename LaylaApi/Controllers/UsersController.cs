using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) 
        { 
            _userService = userService;
        }
        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role != null && role.ToLower() == "admin";
        }
        private int CurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                throw new BadHttpRequestException("data is not Valid");

            var currentUserId = CurrentUserId(); // Extension Method
            var isAdmin = IsAdmin();

            // المستخدم العادي لا يعدل إلا نفسه
            if (!isAdmin && id != currentUserId)
                throw new UnauthorizedAccessException("Unauthorized access");

            var result = await _userService.UpdateAsync(id,currentUserId, dto, isAdmin, ct);

            if (result is null)
               throw new KeyNotFoundException("user not found");

            return Ok(ApiResponse<UserDto>.Ok(result));
        }

        [HttpPatch("{id:int}/email")]
        [Authorize]
        public async Task<IActionResult> UpdateEmail(int id, [FromBody] UpdateEmailDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = CurrentUserId();
            var isAdmin = IsAdmin();

            // المستخدم العادي لا يغير إلا بريده
            if (!isAdmin && id != currentUserId)
                throw new UnauthorizedAccessException("Unauthorized access");

            var result = await _userService.UpdateEmailAsync(id, currentUserId, isAdmin, dto.NewEmail, ct);

            if (result is null)
                throw new KeyNotFoundException("user in not exist");

            return Ok(ApiResponse<UserDto>.Ok(result));
        }

        // GET: /api/users/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
        {
            var userId = CurrentUserId();

            var result = await _userService.GetCurrentUserAsync(userId, ct);

            if (result is null)
                throw new KeyNotFoundException ("user is not exist") ;

            return Ok(ApiResponse<UserDto>.Ok(result));
        }
    }
}

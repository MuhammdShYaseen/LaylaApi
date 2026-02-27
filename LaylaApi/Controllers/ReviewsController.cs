using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        private int CurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            return !string.IsNullOrEmpty(role) && role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var all = await _reviewService.GetAllAsync(ct);

            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(all));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var review = await _reviewService.GetByIdAsync(id, ct);

            if (review == null) 
                throw new KeyNotFoundException();

            return Ok(ApiResponse<ReviewDto>.Ok(review));
        }

        [HttpGet("apartment/{apartmentId}")]
        public async Task<IActionResult> GetByApartment(int apartmentId, CancellationToken ct)
        {
            var reviews = await _reviewService.GetByApartmentIdAsync(apartmentId, ct);

            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, CancellationToken ct)
        {
            var reviews = await _reviewService.GetByUserIdAsync(userId, ct);

            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(reviews));
        }

        [HttpGet("apartment/{apartmentId}/average")]
        public async Task<IActionResult> GetAverageRating(int apartmentId, CancellationToken ct)
        {
            var result = await _reviewService.GetAverageRatingAsync(apartmentId, ct);

            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto, CancellationToken ct)
        {
            var result = await _reviewService.AddAsync(dto, CurrentUserId(), IsAdmin(), ct);

            return Ok(ApiResponse<ReviewDto>.Ok(result, "Review created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewCreateDto dto, CancellationToken ct)
        {
            var result = await _reviewService.UpdateAsync(id, dto, CurrentUserId(), IsAdmin(), ct);

            return Ok(ApiResponse<ReviewDto>.Ok(result, "Review updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _reviewService.DeleteAsync(id, CurrentUserId(), IsAdmin(), ct);

            return Ok(ApiResponse<object>.Ok("Review deleted."));
        }
    }
}

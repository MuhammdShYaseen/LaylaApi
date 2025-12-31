using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
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
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        public ReviewsController( IReviewService reviewService, IBookingService bookingService, IMapper mapper)
        {
            _reviewService = reviewService;
            _bookingService = bookingService;
            _mapper = mapper;
        }
        private int CurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        private bool IsCurrentUserAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return !string.IsNullOrEmpty(role) && role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _reviewService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(all));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return NotFound();
            return Ok(_mapper.Map<ReviewDto>(review));
        }

        [HttpGet("apartment/{apartmentId}")]
        public async Task<IActionResult> GetByApartment(int apartmentId)
        {
            var reviews = await _reviewService.GetByApartmentIdAsync(apartmentId);
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var reviews = await _reviewService.GetByUserIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        [HttpGet("apartment/{apartmentId}/average")]
        public async Task<IActionResult> GetAverageRating(int apartmentId)
        {
            var reviews = await _reviewService.GetByApartmentIdAsync(apartmentId);
            if (reviews == null || !reviews.Any())
                return Ok(new { average = 0.0, count = 0 });

            var avg = reviews.Average(r => r.Rating);
            return Ok(new { average = Math.Round(avg, 2), count = reviews.Count() });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.ApartmentId <= 0)
                return BadRequest(new { message = "ApartmentId is required." });

            // Must have completed booking for this apartment
            var bookings = await _bookingService.GetByUserIdAsync(userId);

            var hadBooking = bookings.Any(b =>
                b.ApartmentId == dto.ApartmentId &&
                b.Status != Booking.BookingStatus.CancelledByRenter &&
                (b.Status == Booking.BookingStatus.Completed || b.EndDate <= DateTime.UtcNow));

            if (!hadBooking)
                return Forbid("You can only review an apartment you have booked and stayed in.");

            // Prevent duplicate review
            var exists = await _reviewService.ExistsAsync(userId, dto.ApartmentId);
            if (exists)
                return BadRequest(new { message = "You have already reviewed this apartment." });

            // Create entity
            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            var created = await _reviewService.AddAsync(review);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ReviewDto>(created));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewCreateDto dto)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var existing = await _reviewService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // Only owner or admin can edit
            if (existing.UserId != userId && !IsCurrentUserAdmin())
                return Forbid("You can only update your own review.");

            // Validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest(new { message = "Rating must be between 1 and 5." });

            // Apply only allowed changes
            existing.Rating = dto.Rating;
            existing.Comment = dto.Comment;

            var updated = await _reviewService.UpdateAsync(id, existing);
            if (updated == null)
                return BadRequest(new { message = "Could not update review." });

            return Ok(_mapper.Map<ReviewDto>(updated));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var existing = await _reviewService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (existing.UserId != userId && !IsCurrentUserAdmin())
                return Forbid("You can only delete your own review.");

            var success = await _reviewService.DeleteAsync(id);
            if (!success) return BadRequest(new { message = "Could not delete review." });

            return Ok(new { message = "Review deleted." });
        }
    }
}

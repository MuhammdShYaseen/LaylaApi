using AutoMapper;
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

        private bool IsAdmin()
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
            if (review == null) 
                throw new KeyNotFoundException();
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
            var result = await _reviewService.AddAsync(dto, CurrentUserId(), IsAdmin());

            return Ok(ApiResponse<ReviewDto>.Ok(result, "Review created successfully."));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewCreateDto dto)
        {
            var result = await _reviewService.UpdateAsync(id, dto, CurrentUserId(), IsAdmin());

            return Ok(ApiResponse<ReviewDto>.Ok(result, "Review updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteAsync(id, CurrentUserId(), IsAdmin());
            return Ok(ApiResponse<object>.Ok("Review deleted."));
        }
    }
}

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
    public class ApartmentsController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;

        public ApartmentsController(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        // 🔐 إضافة شقة — فقط للمستخدم المسجّل
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddApartment([FromBody] CreateApartmentDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _apartmentService.AddAsync(dto, userId);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateApartment(int id, [FromBody] CreateApartmentDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _apartmentService.UpdateAsync(id, dto, userId);

            if (result == null)
                return NotFound("Apartment not found or you do not own it.");

            return Ok(result);
        }

        // 🔍 البحث عن شقق
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword cannot be empty.");

            var result = await _apartmentService.SearchAsync(keyword);

            return Ok(result);
        }

        // 📍 الشقق القريبة من موقع المستخدم
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double distanceKm = 5.0)
        {
            var result = await _apartmentService.GetNearbyAsync(lat, lng, distanceKm);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var success = await _apartmentService.DeleteAsync(id, userId);

            if (!success)
                return BadRequest("Unable to delete apartment or you do not own it.");

            return Ok(new { message = "Apartment deleted successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _apartmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _apartmentService.GetByIdAsync(id);

            if (result == null)
                return NotFound("Apartment not found.");

            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyApartments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _apartmentService.GetByOwnerIdAsync(userId);

            return Ok(result);
        }
    }
}

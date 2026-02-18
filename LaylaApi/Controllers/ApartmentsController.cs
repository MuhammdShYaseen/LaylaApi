using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Services.DynamicApartmentSearchService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApartmentsController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;
        private readonly IApartmentSearchService _dynamicSearch;
        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role != null && role.ToLower() == "admin";
        }
        private int CurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        public ApartmentsController(IApartmentService apartmentService, IApartmentSearchService searchService)
        {
            _apartmentService = apartmentService;
            _dynamicSearch = searchService;
        }

        // 🔐 إضافة شقة — فقط للمستخدم المسجّل
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddApartment([FromBody] CreateApartmentDto dto, CancellationToken ct)
        {

            var result = await _apartmentService.AddAsync(dto, CurrentUserId(), ct);
            
            return Ok(ApiResponse<ApartmentDto>.Ok(result));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateApartment(int id, [FromBody] CreateApartmentDto dto, CancellationToken ct)
        {

            var result = await _apartmentService.UpdateAsync(id, dto, CurrentUserId(), IsAdmin(), ct);

            if (result == null)
                throw new KeyNotFoundException("Apartment not found or you do not own it.");

            return Ok(ApiResponse<ApartmentDto>.Ok(result));
        }

        // 🔍 البحث عن شقق
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new BadHttpRequestException("Keyword cannot be empty.");

            var result = await _apartmentService.SearchAsync(keyword, ct);

            return Ok(ApiResponse<IEnumerable<ApartmentDto>>.Ok(result));
        }

        // 📍 الشقق القريبة من موقع المستخدم
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby(CancellationToken ct, [FromQuery] double lat, [FromQuery] double lng, [FromQuery] double distanceKm = 5.0)
        {
            var result = await _apartmentService.GetNearbyAsync(lat, lng, distanceKm, ct);

            return Ok(ApiResponse<IEnumerable<ApartmentDto>>.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var success = await _apartmentService.DeleteAsync(id, userId, ct);

            if (!success)
                throw new BadHttpRequestException("Unable to delete apartment or you do not own it.");

            return Ok(ApiResponse<object>.Ok( "Apartment deleted successfully."));
        }

        [HttpGet("dynamic")]
        public async Task<ActionResult<PagedResult<ApartmentDto>>> Search([FromQuery] ApartmentSearchRequestDto request, CancellationToken ct)
        {
            var result = await _dynamicSearch.SearchAsync(request, ct);

            return Ok(ApiResponse<PagedResult<ApartmentDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _apartmentService.GetByIdAsync(id, ct);

            if (result == null)
               throw new KeyNotFoundException("Apartment not found.");

            return Ok(ApiResponse<ApartmentDto>.Ok(result));
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyApartments(CancellationToken ct)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _apartmentService.GetByOwnerIdAsync(userId, ct);

            return Ok(ApiResponse<IEnumerable<ApartmentDto>>.Ok(result));
        }
    }
}

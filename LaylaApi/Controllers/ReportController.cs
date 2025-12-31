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
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IApartmentService _apartmentService;
        private readonly IMapper _mapper;
        public ReportController(IReportService reportService, IApartmentService apartmentService, IMapper mapper)
        {
            _reportService = reportService;
            _apartmentService = apartmentService;
            _mapper = mapper;
        }

        private int CurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }

        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _reportService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ReportDto>>(reports));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if (report == null) return NotFound();

            var userId = CurrentUserId();
            if (report.ReporterId != userId && !IsAdmin())
                return Forbid();

            return Ok(_mapper.Map<ReportDto>(report));
        }

        [HttpGet("apartment/{apartmentId}")]
        [Authorize(Roles = "Admin")] // فقط المدير
        public async Task<IActionResult> GetByApartment(int apartmentId)
        {
            var reports = await _reportService.GetByApartmentIdAsync(apartmentId);
            return Ok(_mapper.Map<IEnumerable<ReportDto>>(reports));
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReports()
        {
            var userId = CurrentUserId();
            var reports = await _reportService.GetByReporterIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<ReportDto>>(reports));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ReportCreateDto model)
        {
            var userId = CurrentUserId();
            if (userId == 0)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.ApartmentId <= 0)
                return BadRequest(new { message = "ApartmentId is required." });

            // تأكيد أن الشقة موجودة
            var apartment = await _apartmentService.GetByIdAsync(model.ApartmentId);
            if (apartment == null)
                return NotFound(new { message = "Apartment not found." });

            // منع التبليغ عن شقته الخاصة
            if (apartment.OwnerId == userId)
                return BadRequest(new { message = "You cannot report your own apartment." });

            // منع التبليغ المكرر
            bool exists = await _reportService.ExistsAsync(userId, model.ApartmentId);
            if (exists)
                return BadRequest(new { message = "You have already reported this apartment." });

            // تجهيز الكيان
            var report = _mapper.Map<Report>(model);
            report.ReporterId = userId;
            report.Status = "Pending";
            report.CreatedAt = DateTime.UtcNow;

            // الإضافة
            var created = await _reportService.AddAsync(report);

            // إعادة النتيجة بشكل آمن
            return Ok(_mapper.Map<ReportDto>(created));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var allowed = new[] { "Pending", "Reviewed", "Resolved", "Rejected" };
            if (!allowed.Contains(status))
                return BadRequest(new { message = "Invalid status value." });

            var updated = await _reportService.UpdateStatusAsync(id, status);
            if (updated == null) return NotFound();

            return Ok(_mapper.Map<ReportDto>(updated));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if (report == null) return NotFound();

            var userId = CurrentUserId();

            if (report.ReporterId != userId && !IsAdmin())
                return Forbid();

            var success = await _reportService.DeleteAsync(id);
            if (!success) return BadRequest(new { message = "Could not delete report." });

            return Ok(new { message = "Report deleted." });
        }
    }
}

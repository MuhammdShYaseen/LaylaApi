using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LaylaApi.Models.MainModels.Report;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
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
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _reportService.GetAllAsync(ct);
            return Ok(ApiResponse<IEnumerable<ReportDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _reportService.GetByIdAsync(id, CurrentUserId(), IsAdmin(), ct);

            return Ok(ApiResponse<ReportDto>.Ok(result));
        }

        [HttpGet("apartment/{apartmentId}")]
        [Authorize(Roles = "Admin")] // فقط المدير
        public async Task<IActionResult> GetByApartment(int apartmentId, CancellationToken ct)
        {
            var result = await _reportService.GetByApartmentIdAsync(apartmentId, ct);
            return Ok(ApiResponse<IEnumerable<ReportDto>>.Ok(result));
        }

        [HttpGet("my")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> GetMyReports(CancellationToken ct)
        {
            var result = await _reportService.GetByReporterIdAsync(CurrentUserId(), ct);
            return Ok(ApiResponse<IEnumerable<ReportDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> Create([FromBody] ReportCreateDto model, CancellationToken ct)
        {
            var result = await _reportService.CreateAsync(model, CurrentUserId(), IsAdmin(), ct);

            return Ok(ApiResponse<ReportDto>.Ok(result, "Report submitted successfully."));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status, CancellationToken ct)
        {
            if (!Enum.TryParse<ReportStatus>(status, true, out var newStatus))
                throw new BadHttpRequestException("Invalid report status.");


            var result = await _reportService.UpdateStatusAsync(id, newStatus, ct);

            return Ok(ApiResponse<ReportDto>.Ok(result, "Report status updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ConfirmedEmail")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _reportService.DeleteAsync(id, CurrentUserId(), IsAdmin(), ct);

            return Ok(ApiResponse<object>.Ok("Report deleted."));
        }
    }
}

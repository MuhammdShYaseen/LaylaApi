
using LaylaApi.Models.DtosModels.AdminDashboardDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.AdminDashboardService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController :ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public AdminDashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> Overview(CancellationToken ct)
        {
            var result = await _dashboardService.GetOverviewAsync(ct);
            return Ok(ApiResponse<OverviewDto>.Ok(result));
        }

        [HttpGet("bookings/status")]
        public async Task<IActionResult> BookingStatus(CancellationToken ct)
        {
            var result = await _dashboardService.GetBookingStatusStatsAsync(ct);
            return Ok(ApiResponse<IEnumerable<StatusStatsDto>>.Ok(result));
        }

        [HttpGet("bookings/monthly")]
        public async Task<IActionResult> MonthlyBookings(CancellationToken ct)
        {
            var result = await _dashboardService.GetMonthlyBookingsAsync(ct);
            return Ok(ApiResponse<IEnumerable<MonthlyStatsDto>>.Ok(result));
        }

        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> MonthlyRevenue(CancellationToken ct)
        {
            var result = await _dashboardService.GetMonthlyRevenueAsync(ct);
            return Ok(ApiResponse<IEnumerable<MonthlyRevenueDto>>.Ok(result));
        }
        [HttpGet("apartments/top-booked")]
        public async Task<IActionResult> TopBooked(CancellationToken ct)
        {
            var result = await _dashboardService.GetTopBookedApartmentsAsync(ct);
            return Ok(ApiResponse<IEnumerable<TopApartmentDto>>.Ok(result));
        }

        [HttpGet("apartments/top-rated")]
        public async Task<IActionResult> TopRated(CancellationToken ct)
        {
            var result = await _dashboardService.GetTopRatedApartmentsAsync(ct);
            return Ok(ApiResponse<IEnumerable<TopRatedApartmentDto>>.Ok(result));
        }

        [HttpGet("users/top-renters")]
        public async Task<IActionResult> TopRenters(CancellationToken ct)
        {
            var result = await _dashboardService.GetTopRentersAsync(ct);
            return Ok(ApiResponse<IEnumerable<TopUserDto>>.Ok(result));
        }

        [HttpGet("users/top-owners")]
        public async Task<IActionResult> TopOwners(CancellationToken ct)
        {
            var result = await _dashboardService.GetTopOwnersAsync(ct);
            return Ok(ApiResponse<IEnumerable<TopUserDto>>.Ok(result));
        }

        [HttpGet("reports/monthly")]
        public async Task<IActionResult> MonthlyReports(CancellationToken ct)
        {
            var result = await _dashboardService.GetMonthlyReportsAsync(ct);
            return Ok(ApiResponse<IEnumerable<MonthlyStatsDto>>.Ok(result));
        }

        [HttpGet("reports/today")]
        public async Task<IActionResult> TodayReports(CancellationToken ct)
        {
            var result = await _dashboardService.GetTodayReportsAsync(ct);
            return Ok(ApiResponse<int>.Ok(result));
        }
    }
}

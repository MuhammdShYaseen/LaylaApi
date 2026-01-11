
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
        public async Task<IActionResult> Overview()
        {
            var result = await _dashboardService.GetOverviewAsync();
            return Ok(ApiResponse<OverviewDto>.Ok(result));
        }

        [HttpGet("bookings/status")]
        public async Task<IActionResult> BookingStatus()
        {
            var result = await _dashboardService.GetBookingStatusStatsAsync();
            return Ok(ApiResponse<IEnumerable<StatusStatsDto>>.Ok(result));
        }

        [HttpGet("bookings/monthly")]
        public async Task<IActionResult> MonthlyBookings()
        {
            var result = await _dashboardService.GetMonthlyBookingsAsync();
            return Ok(ApiResponse<IEnumerable<MonthlyStatsDto>>.Ok(result));
        }

        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> MonthlyRevenue()
        {
            var result = await _dashboardService.GetMonthlyRevenueAsync();
            return Ok(ApiResponse<IEnumerable<MonthlyRevenueDto>>.Ok(result));
        }
        [HttpGet("apartments/top-booked")]
        public async Task<IActionResult> TopBooked()
        {
            var result = await _dashboardService.GetTopBookedApartmentsAsync();
            return Ok(ApiResponse<IEnumerable<TopApartmentDto>>.Ok(result));
        }

        [HttpGet("apartments/top-rated")]
        public async Task<IActionResult> TopRated()
        {
            var result = await _dashboardService.GetTopRatedApartmentsAsync();
            return Ok(ApiResponse<IEnumerable<TopRatedApartmentDto>>.Ok(result));
        }

        [HttpGet("users/top-renters")]
        public async Task<IActionResult> TopRenters()
        {
            var result = await _dashboardService.GetTopRentersAsync();
            return Ok(ApiResponse<IEnumerable<TopUserDto>>.Ok(result));
        }

        [HttpGet("users/top-owners")]
        public async Task<IActionResult> TopOwners()
        {
            var result = await _dashboardService.GetTopOwnersAsync();
            return Ok(ApiResponse<IEnumerable<TopUserDto>>.Ok(result));
        }

        [HttpGet("reports/monthly")]
        public async Task<IActionResult> MonthlyReports()
        {
            var result = await _dashboardService.GetMonthlyReportsAsync();
            return Ok(ApiResponse<IEnumerable<MonthlyStatsDto>>.Ok(result));
        }

        [HttpGet("reports/today")]
        public async Task<IActionResult> TodayReports()
        {
            var result = await _dashboardService.GetTodayReportsAsync();
            return Ok(ApiResponse<int>.Ok(result));
        }
    }
}

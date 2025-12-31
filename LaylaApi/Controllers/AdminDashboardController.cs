
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
            => Ok(await _dashboardService.GetOverviewAsync());

        [HttpGet("bookings/status")]
        public async Task<IActionResult> BookingStatus()
            => Ok(await _dashboardService.GetBookingStatusStatsAsync());

        [HttpGet("bookings/monthly")]
        public async Task<IActionResult> MonthlyBookings()
            => Ok(await _dashboardService.GetMonthlyBookingsAsync());

        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> MonthlyRevenue()
            => Ok(await _dashboardService.GetMonthlyRevenueAsync());

        [HttpGet("apartments/top-booked")]
        public async Task<IActionResult> TopBooked()
            => Ok(await _dashboardService.GetTopBookedApartmentsAsync());

        [HttpGet("apartments/top-rated")]
        public async Task<IActionResult> TopRated()
            => Ok(await _dashboardService.GetTopRatedApartmentsAsync());

        [HttpGet("users/top-renters")]
        public async Task<IActionResult> TopRenters()
            => Ok(await _dashboardService.GetTopRentersAsync());

        [HttpGet("users/top-owners")]
        public async Task<IActionResult> TopOwners()
            => Ok(await _dashboardService.GetTopOwnersAsync());

        [HttpGet("reports/monthly")]
        public async Task<IActionResult> MonthlyReports()
            => Ok(await _dashboardService.GetMonthlyReportsAsync());

        [HttpGet("reports/today")]
        public async Task<IActionResult> TodayReports()
            => Ok(await _dashboardService.GetTodayReportsAsync());
    }
}

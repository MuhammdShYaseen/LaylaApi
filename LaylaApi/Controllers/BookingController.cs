using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // 🟦 إنشاء حجز جديد
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _bookingService.AddAsync(model, userId);

            return Ok(ApiResponse<BookingDto>.Ok(result));
        }

        // 🔍 عرض الحجوزات الخاصة بالمستخدم
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var renterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _bookingService.GetByUserIdAsync(renterId);

            return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok(result));
        }

        [HttpGet("owner")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> OwnerBookings()
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _bookingService.GetBookingsForOwnerAsync(ownerId);

            return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok(result.OrderByDescending(b => b.StartDate).ToList()));
        }

        // 📅 التحقق من توفر التاريخ
        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> CheckAvailability([FromQuery] int apartmentId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            bool available = await _bookingService.IsDateAvailableAsync(apartmentId, start, end);

            return Ok(ApiResponse<bool>.Ok(available));
        }

        // ❌ إلغاء حجز
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelByUser(int id)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            bool success = await _bookingService.CancelAsync(id, UserId);

            if (!success)
                throw new KeyNotFoundException();

            return Ok(ApiResponse<object>.Ok("Booking cancelled successfully"));

        }
        [HttpDelete("{id}/owner-cancel")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> CancelByOwner(int id, [FromQuery] string? reason = null)
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var success = await _bookingService.CancelByOwnerAsync(id, ownerId, reason);

            if (!success) 
                throw new BadHttpRequestException("Cannot cancel this booking");

            return Ok(ApiResponse<object>.Ok("Booking cancelled by owner"));
        }

        // 🔄 تحديث حالة الحجز (يستخدمها صاحب الشقة أو Admin)
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            if (!Enum.TryParse<BookingStatus>(status, true, out var newStatus))
                throw new BadHttpRequestException("Invalid booking status");

            var result = await _bookingService.UpdateStatusAsync(id, newStatus, userId, isAdmin);

            if (result == null)
                throw new KeyNotFoundException("Booking not found or access denied");

            return Ok(ApiResponse<BookingDto>.Ok(result));
        }

        [HttpGet("calendar/{apartmentId}")]
        public async Task<IActionResult> GetCalendar(int apartmentId) //انقل كل المنطق الى السيرفس
        {
            var calendarEvents = await _bookingService.GetCalendarAsync(apartmentId);

            return Ok(ApiResponse<IEnumerable<CalendarEventDto>>.Ok(calendarEvents));
        }

       
    }
}

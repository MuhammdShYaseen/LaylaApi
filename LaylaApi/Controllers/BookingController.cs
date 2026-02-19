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
    [Authorize(Policy = "ConfirmedEmail")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        private bool IsAdmin()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return role != null && role.ToLower() == "admin";
        }

        private int CurrentUserId()
        {
           return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        // 🟦 إنشاء حجز جديد
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto model)
        {
            var result = await _bookingService.AddAsync(model, CurrentUserId());

            return Ok(ApiResponse<BookingDto>.Ok(result));
        }

        // 🔍 عرض الحجوزات الخاصة بالمستخدم
        [HttpGet("my")]
        public async Task<IActionResult> MyBookings()
        {

            var result = await _bookingService.GetByUserIdAsync(CurrentUserId());

            return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok(result));
        }

        [HttpGet("owner")]
        public async Task<IActionResult> OwnerBookings()
        {
            var result = await _bookingService.GetBookingsForOwnerAsync(CurrentUserId());

            return Ok(ApiResponse<IEnumerable<BookingDto>>.Ok(result.OrderByDescending(b => b.StartDate).ToList()));
        }

        // 📅 التحقق من توفر التاريخ
        [HttpGet("check")]
        public async Task<IActionResult> CheckAvailability([FromQuery] int apartmentId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            bool available = await _bookingService.IsDateAvailableAsync(apartmentId, start, end);

            return Ok(ApiResponse<bool>.Ok(available));
        }

        // ❌ إلغاء حجز
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelByUser(int id)
        {

            bool success = await _bookingService.CancelAsync(id, CurrentUserId());

            if (!success)
                throw new KeyNotFoundException();

            return Ok(ApiResponse<object>.Ok("Booking cancelled successfully"));

        }
        [HttpDelete("{id}/owner-cancel")]
        public async Task<IActionResult> CancelByOwner(int id, [FromQuery] string? reason = null)
        {

            var success = await _bookingService.CancelByOwnerAsync(id, CurrentUserId(), reason);

            if (!success) 
                throw new BadHttpRequestException("Cannot cancel this booking");

            return Ok(ApiResponse<object>.Ok("Booking cancelled by owner"));
        }

        // 🔄 تحديث حالة الحجز (يستخدمها صاحب الشقة أو Admin)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {

            if (!Enum.TryParse<BookingStatus>(status, true, out var newStatus))
                throw new BadHttpRequestException("Invalid booking status");

            var result = await _bookingService.UpdateStatusAsync(id, newStatus, CurrentUserId(), IsAdmin());

            if (result == null)
                throw new KeyNotFoundException("Booking not found or access denied");

            return Ok(ApiResponse<BookingDto>.Ok(result));
        }

        [HttpPut("{id}/UpdateBooking")]
        public async Task<IActionResult> UpdateBooking (int id, [FromBody] CreateBookingDto dto)
        {
            var result = await _bookingService.UpdateAsync(id,dto, CurrentUserId(), IsAdmin());
            return Ok(ApiResponse<BookingDto>.Ok(result!));
        }

        [HttpGet("calendar/{apartmentId}")]
        public async Task<IActionResult> GetCalendar(int apartmentId) //انقل كل المنطق الى السيرفس
        {
            var calendarEvents = await _bookingService.GetCalendarAsync(apartmentId);

            return Ok(ApiResponse<IEnumerable<CalendarEventDto>>.Ok(calendarEvents));
        }

       
    }
}

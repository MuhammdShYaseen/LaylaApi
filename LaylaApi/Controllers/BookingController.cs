using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
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

            return Ok(result);          
        }

        // 🔍 عرض الحجوزات الخاصة بالمستخدم
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var renterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _bookingService.GetByUserIdAsync(renterId);

            return Ok(result);
        }

        [HttpGet("owner")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> OwnerBookings()
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var bookings = await _bookingService.GetBookingsForOwnerAsync(ownerId);
            
            return Ok(bookings.OrderByDescending(b => b.StartDate));
        }

        // 📅 التحقق من توفر التاريخ
        [HttpGet("check")]
        [Authorize]
        public async Task<IActionResult> CheckAvailability(int apartmentId, DateTime start, DateTime end)
        {
            bool available = await _bookingService.IsDateAvailableAsync(apartmentId, start, end);

            return Ok(new { available });
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

            return Ok(new { message = "Booking cancelled successfully" });
           
        }
        [HttpDelete("{id}/owner-cancel")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> CancelByOwner(int id, [FromQuery] string? reason = null)
        {
            var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var success = await _bookingService.CancelByOwnerAsync(id, ownerId, reason);

            if (!success) 
                throw new BadHttpRequestException("Cannot cancel this booking");

            return Ok(new { message = "Booking cancelled by owner" });
        }

        // 🔄 تحديث حالة الحجز (يستخدمها صاحب الشقة أو Admin)
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                throw new UnauthorizedAccessException();

            var booking = await _bookingService.GetEntityByIdAsync(id);

            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            if (booking.Apartment == null)
                throw new KeyNotFoundException("Apartment not found");

            // Only the owner of the apartment can update the status
            if (booking.Apartment.OwnerId != userId && !User.IsInRole("Admin"))
               throw new UnauthorizedAccessException();

            if (!Enum.TryParse<BookingStatus>(status, true, out var newStatus))
                throw new BadHttpRequestException("Invalid booking status");

            var result = await _bookingService.UpdateStatusAsync(id, newStatus);

            if (result == null)
               throw new KeyNotFoundException();

            return Ok(result);
        }

        [HttpGet("calendar/{apartmentId}")]
        public async Task<IActionResult> GetCalendar(int apartmentId)
        {
            var bookingsDto = await _bookingService.GetByApartmentIdAsync(apartmentId);

            var calendarEvents = bookingsDto
                .Where(b => b.Status != BookingStatus.CancelledByRenter
                         && b.Status != BookingStatus.CancelledByOwner)
                .Select(b => new
                {
                    id = b.Id,
                    title = b.Status,   // أو ثابت "Booked"
                    start = b.StartDate.ToString("yyyy-MM-dd"),
                    end = b.EndDate.ToString("yyyy-MM-dd"),
                    status = b.Status,
                    color = GetStatusColor(b.Status.ToString()) // اختياري
                });

            return Ok(calendarEvents);
        }

        private string GetStatusColor(string status)
        {
            return status switch
            {
                "Pending" => "#FACC15", // أصفر
                "Accepted" => "#3B82F6", // أزرق
                "Confirmed" => "#16A34A", // أخضر
                "Completed" => "#10B981", // أخضر فاتح
                "Cancelled" => "#EF4444", // أحمر
                "CancelledByOwner" => "#DC2626",
                _ => "#6B7280"              // رمادي
            };
        }
    }
}

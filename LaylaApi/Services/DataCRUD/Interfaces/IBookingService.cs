using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllAsync();//
        Task<BookingDto?> GetByIdAsync(int id);//
        Task<Booking?> GetEntityByIdAsync(int id);
        Task<IEnumerable<BookingDto>> GetBookingsForOwnerAsync(int ownerId);
        Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<BookingDto>> GetByApartmentIdAsync(int apartmentId);
        Task<BookingDto?> UpdateAsync(int bookingId, CreateBookingDto dto, int renterId, bool isAdmin);       
        Task<bool> IsDateAvailableAsync(int apartmentId, DateTime startDate, DateTime endDate);
        Task<BookingDto> AddAsync(CreateBookingDto booking,int UserID);
        Task<BookingDto?> UpdateStatusAsync(int bookingId, BookingStatus newStatus, int actorUserId, bool isAdmin);
        Task<IEnumerable<CalendarEventDto>> GetCalendarAsync(int apartmentId);
        Task<bool> CancelAsync(int id, int renterId);
        Task<bool> DeleteAsync(int id);
        Task<bool> CancelByOwnerAsync(int bookingId, int ownerId, string? reason = null);

    }
}

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
        Task<IEnumerable<BookingDto>> GetBookingsForOwnerAsync(int ownerId, CancellationToken ct);
        Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId, CancellationToken ct);
        Task<IEnumerable<BookingDto>> GetByApartmentIdAsync(int apartmentId);
        Task<BookingDto?> UpdateAsync(int bookingId, CreateBookingDto dto, int renterId, bool isAdmin, CancellationToken ct);       
        Task<bool> IsDateAvailableAsync(int apartmentId, DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<BookingDto> AddAsync(CreateBookingDto booking,int UserID, CancellationToken ct);
        Task<BookingDto?> UpdateStatusAsync(int bookingId, BookingStatus newStatus, int actorUserId, bool isAdmin, CancellationToken ct);
        Task<IEnumerable<CalendarEventDto>> GetCalendarAsync(int apartmentId);
        Task<bool> CancelAsync(int id, int renterId, CancellationToken ct);
        Task<bool> DeleteAsync(int id);
        Task<bool> CancelByOwnerAsync(CancellationToken ct, int bookingId, int ownerId, string? reason = null);

    }
}

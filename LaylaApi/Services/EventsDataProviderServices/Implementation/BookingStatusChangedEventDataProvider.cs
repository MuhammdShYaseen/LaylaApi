using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class BookingStatusChangedEventDataProvider : IEventDataProvider<BookingStatusChangedEvent, BookingStatusChangedEventDto>
    {
        private readonly IRepository<Booking> _bookingRepository;

        public BookingStatusChangedEventDataProvider(IRepository<Booking> bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public async Task<BookingStatusChangedEventDto> GetDataAsync(BookingStatusChangedEvent @event, CancellationToken ct)
        {
            return await _bookingRepository
             .Query()
             .AsNoTracking()
             .Where(b => b.Guid == @event.BookingGuid)
             .Select(b => new BookingStatusChangedEventDto
             {
                 BookingId = b.Guid,
                 NewStatus = @event.NewStatus,

                 ApartmentTitle = b.Apartment!.Title,

                 OwnerId = b.Apartment.OwnerId,
                 OwnerEmail = b.Apartment.Owner!.Email!.Value,
                 OwnerLang = b.Apartment.Owner.Lang!.Code,

                 RenterId = b.UserId,
                 RenterEmail = b.User!.Email!.Value,
                 RenterLang = b.User.Lang!.Code
             })
             .SingleAsync(ct);
        }
    }
}

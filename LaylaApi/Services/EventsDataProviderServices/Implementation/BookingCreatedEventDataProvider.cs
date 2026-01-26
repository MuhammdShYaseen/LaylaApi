using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class BookingCreatedEventDataProvider : IEventDataProvider<BookingCreatedEvent, BookingCreatedEventDto>
    {
        private readonly IRepository<Booking> _repository;

        public BookingCreatedEventDataProvider(IRepository<Booking> repository)
        {
            _repository = repository;
        }
        public async Task<BookingCreatedEventDto> GetDataAsync(BookingCreatedEvent @event, CancellationToken ct)
        {
            return await _repository.Query(false)
            .AsNoTracking()
            .Where(b => b.Guid == @event.BookingGuid)
            .Select(b => new BookingCreatedEventDto
            {
                ApartmentTitle = b.Apartment!.Title,

                StartDate = b.StartDate,
                EndDate = b.EndDate,

                OwnerId = b.Apartment.OwnerId,
                OwnerEmail = b.Apartment.Owner!.Email,
                OwnerLanguage = b.Apartment.Owner.Lang,

                RenterId = b.UserId,
                RenterEmail = b.User!.Email,
                RenterName = b.User.FullName,
                RenterLanguage = b.User.Lang
            })
            .SingleAsync(ct);
        }
    }
}

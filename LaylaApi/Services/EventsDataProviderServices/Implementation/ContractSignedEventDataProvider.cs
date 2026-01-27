using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class ContractSignedEventDataProvider : IEventDataProvider<ContractSignedEvent, ContractSignedEventDto>
    {
        private readonly IRepository<Contract> _contractRepository;

        public ContractSignedEventDataProvider(IRepository<Contract> contractRepository)
        {
            _contractRepository = contractRepository;
        }
        public async Task<ContractSignedEventDto> GetDataAsync(ContractSignedEvent @event, CancellationToken ct)
        {
            return await _contractRepository
            .Query(false)
            .AsNoTracking()
            .Where(c => c.Guid == @event.ContractGuid)
            .Select(c => new ContractSignedEventDto
            {
                ContractId = c.Guid,
                BookingId = c.Booking!.Id,

                IsOwnerSigner = @event.IsOwner,
                IsFullySigned = @event.IsFullySigned,

                ApartmentTitle = c.Booking.Apartment!.Title,

                OwnerId = c.Booking.Apartment.OwnerId,
                OwnerEmail = c.Booking.Apartment.Owner!.Email,
                OwnerLang = c.Booking.Apartment.Owner.Lang,

                RenterId = c.Booking.UserId,
                RenterEmail = c.Booking.User!.Email,
                RenterLang = c.Booking.User.Lang
            })
            .SingleAsync(ct);
        }
    }
}

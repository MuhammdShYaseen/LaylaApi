using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class ContractCreatedEventDataProvider : IEventDataProvider<ContractCreatedEvent, ContractCreatedEventDto>
    {
        private readonly IRepository<Contract> _contractRepository;

        public ContractCreatedEventDataProvider(IRepository<Contract> contractRepository)
        {
            _contractRepository = contractRepository;
        }
        public async Task<ContractCreatedEventDto> GetDataAsync(ContractCreatedEvent @event, CancellationToken ct)
        {
            return await _contractRepository
            .Query(false)
            .AsNoTracking()
            .Where(c => c.Guid == @event.ContractGuid)
            .Select(c => new ContractCreatedEventDto
            {
                ContractId = c.Guid,
                BookingId = c.Booking!.Id,

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

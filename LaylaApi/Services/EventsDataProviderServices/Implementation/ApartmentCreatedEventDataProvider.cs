using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class ApartmentCreatedEventDataProvider : IEventDataProvider<ApartmentCreatedEvent, ApartmentCreatedEventDto>
    {
        private readonly IRepository<Apartment> _repository;
        public ApartmentCreatedEventDataProvider(IRepository<Apartment> repository)
        {
            _repository = repository;
        }
        public async Task<ApartmentCreatedEventDto> GetDataAsync(ApartmentCreatedEvent @event, CancellationToken ct)
        {
            return await _repository.Query(false)
                .AsNoTracking()
                .Where(a => a.Guid == @event.ApartmentGuid)
                .Select(a => new ApartmentCreatedEventDto
                {
                    ApartmentId = a.Guid,
                    ApartmentTitle = a.Title,
                    OwnerId = a.OwnerId,
                    OwnerEmail = a.Owner!.Email,
                    OwnerLang = a.Owner.Lang
                })
                .SingleAsync(ct);
        }
    }
}

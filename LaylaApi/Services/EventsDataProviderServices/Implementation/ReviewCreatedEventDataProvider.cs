using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class ReviewCreatedEventDataProvider : IEventDataProvider<ReviewCreatedEvent, ReviewCreatedEventDto>
    {
        private readonly IRepository<Review> _reviews;

        public ReviewCreatedEventDataProvider(IRepository<Review> reviews)
        {
            _reviews = reviews;
        }

        public async Task<ReviewCreatedEventDto> GetDataAsync(ReviewCreatedEvent @event, CancellationToken ct)
        {
            return await _reviews.Query(false)
                .AsNoTracking()
                .Where(r => r.Guid == @event.ReviewGuid)
                .Select(r => new ReviewCreatedEventDto
                {
                    Rating = r.Rating,
                    OwnerId = r.Apartment!.OwnerId,
                    OwnerLang = r.Apartment!.Owner!.Lang!.ToString()
                })
                .SingleAsync(ct);
        }
    }
}

using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Options;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class UserRegisteredEventDataProvider : IEventDataProvider<UserRegisteredEvent, UserRegisteredEventDto>
    {
        private readonly IRepository<User> _users;
        private readonly FrontendOptions _frontendOptions;

        public UserRegisteredEventDataProvider(
            IRepository<User> users,
            IOptions<FrontendOptions> frontendOptions)
        {
            _users = users;
            _frontendOptions = frontendOptions.Value;
        }

        public async Task<UserRegisteredEventDto> GetDataAsync(
            UserRegisteredEvent @event,
            CancellationToken ct)
        {
            return await _users.Query(false)
                .AsNoTracking()
                .Where(u => u.Guid == @event.UserGuid)
                .Select(u => new UserRegisteredEventDto
                {
                    FullName = u.FullName,
                    Email = u.Email,
                    Lang = u.Lang,
                    VerificationUrl =
                        $"{_frontendOptions.Verify}{@event.Token}"
                })
                .SingleAsync(ct);
        }
    }
}

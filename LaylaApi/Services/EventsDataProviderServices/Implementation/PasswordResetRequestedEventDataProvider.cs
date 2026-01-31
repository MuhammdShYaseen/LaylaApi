using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class PasswordResetRequestedEventDataProvider : IEventDataProvider<PasswordResetRequestedEvent, PasswordResetRequestedEventDto>
    {
        private readonly IRepository<User> _users;

        public PasswordResetRequestedEventDataProvider(IRepository<User> users)
        {
            _users = users;
        }
        public async Task<PasswordResetRequestedEventDto> GetDataAsync(PasswordResetRequestedEvent @event, CancellationToken ct)
        {
            return await _users.Query(false)
            .AsNoTracking()
            .Where(u => u.Guid == @event.UserGuid)
            .Select(u => new PasswordResetRequestedEventDto
            {
                UserId = u.Id,
                Email = u.Email!.Value,
                Lang = u.Lang!.ToString(),
                Token = @event.Token
            })
            .SingleAsync(ct);
        }
    }
}

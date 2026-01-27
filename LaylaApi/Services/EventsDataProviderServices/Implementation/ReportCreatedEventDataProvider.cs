using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.EventsDataProviderServices.Implementation
{
    public class ReportCreatedEventDataProvider : IEventDataProvider<ReportCreatedEvent, ReportCreatedEventDto>
    {
        private readonly IRepository<Report> _reports;

        public ReportCreatedEventDataProvider(IRepository<Report> reports)
        {
            _reports = reports;
        }
        public async Task<ReportCreatedEventDto> GetDataAsync(ReportCreatedEvent @event, CancellationToken ct)
        {
            return await _reports.Query(false)
            .AsNoTracking()
            .Where(r => r.Guid == @event.ReportGuid)
            .Select(r => new ReportCreatedEventDto
            {
                ApartmentId = r.ApartmentId,
                ReporterId = r.ReporterId
            })
            .SingleAsync(ct);
        }
    }
}

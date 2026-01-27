using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ReportCreatedEvent : IEvent
    {
        public Guid ReportGuid { get; }
        public ReportCreatedEvent(Guid reportGuid)
        {
            ReportGuid = reportGuid;
        }
    }
}

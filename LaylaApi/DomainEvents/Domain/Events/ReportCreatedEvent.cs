using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ReportCreatedEvent : IEvent
    {
        public Report _report { get; }
        public ReportCreatedEvent(Report report)
        {
           _report = report;
        }
    }
}

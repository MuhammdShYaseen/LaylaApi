using LaylaApi.Models.MainModels;
using System.ComponentModel.DataAnnotations;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ApartmentCreatedEvent : IEvent
    {
        public Apartment Apartment { get; }

        public ApartmentCreatedEvent(Apartment apartment)
        {
            Apartment = apartment;
        }
    }
}

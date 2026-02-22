using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.MainDtos;

namespace LaylaApi.Models.MainModels
{
    public class Booking : Entity
    {
        public enum BookingStatus
        {
            Pending = 0,
            Accepted = 1,
            Confirmed = 2,
            Rejected = 3,
            CancelledByRenter = 4,
            CancelledByOwner = 5,
            Completed = 6,
        }

        [Required]
        public int ApartmentId { get; private set; }

        [ForeignKey("ApartmentId")]
        public Apartment Apartment { get; set; } = null!;

        [Required]
        public int UserId { get; private set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; private set; }

        [Required]
        public DateTime EndDate { get; private set; }

        [Required]
        public BookingStatus Status { get; private set; } = BookingStatus.Pending; // Pending, Confirmed, Cancelled
        
        public Contract? Contract { get; set; }
        public Payment? Payment { get; set; }
        public static Booking Create(int apartmentId, int renterId, DateTime startDate, DateTime endDate)

        {
            var booking = new Booking
            {
                ApartmentId = apartmentId,
                UserId = renterId,
                StartDate = startDate,
                EndDate = endDate,
                Status = BookingStatus.Pending
            };
            booking.AddDomainEvent(new BookingCreatedEvent(booking.Guid));
            return booking;
        }

        public void ChangeStatus(BookingStatus newStatus)
        {
            if (Status == newStatus)
                return;


            if (!IsValidStatusTransition(Status, newStatus))
                throw new InvalidOperationException("Invalid status transition");

            var oldStatus = Status;

            Status = newStatus;

            Touch();

            AddDomainEvent(new BookingStatusChangedEvent(this.Guid, newStatus));
        }

        private static bool IsValidStatusTransition(BookingStatus current, BookingStatus next)
        {
            return current switch
            {
                BookingStatus.Pending =>
                    next is BookingStatus.Accepted
                        or BookingStatus.CancelledByOwner
                        or BookingStatus.CancelledByRenter,

                BookingStatus.Accepted =>
                    next is BookingStatus.Confirmed
                        or BookingStatus.CancelledByOwner
                        or BookingStatus.CancelledByRenter,

                BookingStatus.Confirmed =>
                    next is BookingStatus.Completed
                        or BookingStatus.CancelledByOwner,

                _ => false
            };
        }

        public void Updated(CreateBookingDto dto)
        {
            Touch();
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
            ApartmentId = dto.ApartmentId;
        }
    }
}
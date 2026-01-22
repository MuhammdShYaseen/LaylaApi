using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.Models.MainModels
{
    public class Booking : Entity
    {
        public enum BookingStatus
        {
            Pending,
            Accepted,
            Confirmed,
            Rejected,
            CancelledByRenter,
            CancelledByOwner,
            Completed
        }

        [Required]
        public int ApartmentId { get; set; }

        [ForeignKey("ApartmentId")]
        public Apartment? Apartment { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending; // Pending, Confirmed, Cancelled
        
        public Contract? Contract { get; set; }
        public Payment? Payment { get; set; }
        public static Booking Create(Apartment apartment, User renter, DateTime startDate, DateTime endDate)

        {
            var booking = new Booking
            {
                Apartment = apartment,
                ApartmentId = apartment.Id,
                User = renter,
                UserId = renter.Id,
                StartDate = startDate,
                EndDate = endDate,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            booking.AddDomainEvent(new BookingCreatedEvent(booking));
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

            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new BookingStatusChangedEvent(
                bookingId: Id,
                oldStatus: oldStatus,
                newStatus: newStatus,
                apartmentId: Apartment!.Id,
                apartmentTitle: Apartment.Title,
                ownerId: Apartment.Owner!.Id,
                ownerEmail: Apartment.Owner.Email,
                ownerLang: Apartment.Owner.Lang,
                renterId: User!.Id,
                renterEmail: User.Email,
                renterLang: User.Lang,
                startDate: StartDate,
                endDate: EndDate
            ));
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
    }
}
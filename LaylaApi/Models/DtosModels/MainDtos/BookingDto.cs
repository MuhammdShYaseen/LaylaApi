using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class BookingDto
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }
        public string? ApartmentTitle { get; set; }
        public string? ApartmentAddress { get; set; }

        public int UserId { get; set; }
        public string? UserFullName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}

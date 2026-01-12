using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class CalendarEventDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public BookingStatus Status { get; set; }
        public string? Color { get; set; }

    }
}

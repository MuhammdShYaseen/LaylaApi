using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Models.DtosModels.AdminDashboardDtos
{
    public class StatusStatsDto
    {
        public BookingStatus Status { get; set; }
        public int Count { get; set; }
    }
}

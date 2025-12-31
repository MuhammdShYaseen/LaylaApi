namespace LaylaApi.Models.DtosModels.AdminDashboardDtos
{
    public class OverviewDto
    {
        public int TotalUsers { get; set; }
        public int TotalApartments { get; set; }
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public int TotalReports { get; set; }
        public decimal TotalRevenue { get; set; }

        public int NewUsersToday { get; set; }
        public int NewApartmentsToday { get; set; }
        public int NewBookingsToday { get; set; }
        public int NewReportsToday { get; set; }
    }
}

namespace LaylaApi.Models.DtosModels.AdminDashboardDtos
{
    public class TopApartmentDto
    {
        public int ApartmentId { get; set; }
        public string ApartmentName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
    }
}

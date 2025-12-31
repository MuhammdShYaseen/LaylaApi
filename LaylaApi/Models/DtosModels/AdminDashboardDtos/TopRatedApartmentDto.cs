namespace LaylaApi.Models.DtosModels.AdminDashboardDtos
{
    public class TopRatedApartmentDto
    {
        public int ApartmentId { get; set; }
        public string ApartmentName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}

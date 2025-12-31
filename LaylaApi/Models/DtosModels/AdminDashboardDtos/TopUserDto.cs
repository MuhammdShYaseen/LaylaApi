namespace LaylaApi.Models.DtosModels.AdminDashboardDtos
{
    public class TopUserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

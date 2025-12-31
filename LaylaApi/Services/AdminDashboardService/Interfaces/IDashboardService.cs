using LaylaApi.Models.DtosModels.AdminDashboardDtos;
using System.Threading.Tasks;

namespace LaylaApi.Services.AdminDashboardService.Interfaces
{
    public interface IDashboardService
    {
        Task<OverviewDto> GetOverviewAsync();
        Task<IEnumerable<StatusStatsDto>> GetBookingStatusStatsAsync();
        Task<IEnumerable<MonthlyStatsDto>> GetMonthlyBookingsAsync();
        Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync();
        Task<IEnumerable<TopApartmentDto>> GetTopBookedApartmentsAsync();
        Task<IEnumerable<TopRatedApartmentDto>> GetTopRatedApartmentsAsync();
        Task<IEnumerable<TopUserDto>> GetTopRentersAsync();
        Task<IEnumerable<TopUserDto>> GetTopOwnersAsync();
        Task<IEnumerable<MonthlyStatsDto>> GetMonthlyReportsAsync();
        Task<int> GetTodayReportsAsync();
    }
}

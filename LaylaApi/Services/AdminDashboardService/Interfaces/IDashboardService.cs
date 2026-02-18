using LaylaApi.Models.DtosModels.AdminDashboardDtos;
using System.Threading.Tasks;

namespace LaylaApi.Services.AdminDashboardService.Interfaces
{
    public interface IDashboardService
    {
        Task<OverviewDto> GetOverviewAsync(CancellationToken ct);
        Task<IEnumerable<StatusStatsDto>> GetBookingStatusStatsAsync(CancellationToken ct);
        Task<IEnumerable<MonthlyStatsDto>> GetMonthlyBookingsAsync(CancellationToken ct);
        Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(CancellationToken ct);
        Task<IEnumerable<TopApartmentDto>> GetTopBookedApartmentsAsync(CancellationToken ct);
        Task<IEnumerable<TopRatedApartmentDto>> GetTopRatedApartmentsAsync(CancellationToken ct);
        Task<IEnumerable<TopUserDto>> GetTopRentersAsync(CancellationToken ct);
        Task<IEnumerable<TopUserDto>> GetTopOwnersAsync(CancellationToken ct);
        Task<IEnumerable<MonthlyStatsDto>> GetMonthlyReportsAsync(CancellationToken ct);
        Task<int> GetTodayReportsAsync(CancellationToken ct);
    }
}

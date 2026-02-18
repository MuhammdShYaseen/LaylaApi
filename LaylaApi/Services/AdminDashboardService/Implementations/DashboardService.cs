using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.AdminDashboardDtos;
using LaylaApi.Services.AdminDashboardService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.AdminDashboardService.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly LaylaContext _context;

        public DashboardService(LaylaContext context)
        {
            _context = context;
        }

        public async Task<OverviewDto> GetOverviewAsync(CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;

            return new OverviewDto
            {
                TotalUsers = await _context.Users.CountAsync(ct),
                TotalApartments = await _context.Apartments.CountAsync(ct),
                TotalBookings = await _context.Bookings.CountAsync(ct),
                TotalReviews = await _context.Reviews.CountAsync(ct),
                TotalReports = await _context.Reports.CountAsync(ct),
                TotalRevenue = await _context.Payments
                    .Where(p => p.Status == "Completed")
                    .SumAsync(p => p.Amount),

                NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt.Date == today, ct),
                NewApartmentsToday = await _context.Apartments.CountAsync(a => a.CreatedAt.Date == today, ct),
                NewBookingsToday = await _context.Bookings.CountAsync(b => b.CreatedAt.Date == today, ct),
                NewReportsToday = await _context.Reports.CountAsync(r => r.CreatedAt.Date == today, ct)
            };
        }

        public async Task<IEnumerable<StatusStatsDto>> GetBookingStatusStatsAsync(CancellationToken ct)
        {
            return await _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new StatusStatsDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<MonthlyStatsDto>> GetMonthlyBookingsAsync(CancellationToken ct)
        {
            return await _context.Bookings
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new MonthlyStatsDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync(CancellationToken ct)
        {
            return await _context.Payments
                .Where(p => p.Status == "Completed")
                .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(p => p.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<TopApartmentDto>> GetTopBookedApartmentsAsync(CancellationToken ct)
        {
            return await _context.Bookings
                .GroupBy(b => b.ApartmentId)
                .Select(g => new TopApartmentDto
                {
                    ApartmentId = g.Key,
                    TotalBookings = g.Count(),
                    ApartmentName = g.First().Apartment!.Title
                })
                .OrderByDescending(x => x.TotalBookings)
                .Take(10)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<TopRatedApartmentDto>> GetTopRatedApartmentsAsync(CancellationToken ct)
        {
            return await _context.Reviews
                .GroupBy(r => r.ApartmentId)
                .Select(g => new TopRatedApartmentDto
                {
                    ApartmentId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    ReviewCount = g.Count(),
                    ApartmentName = g.First().Apartment!.Title
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.ReviewCount)
                .Take(10)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<TopUserDto>> GetTopRentersAsync(CancellationToken ct)
        {
            return await _context.Bookings
                .GroupBy(b => b.UserId)
                .Select(g => new TopUserDto
                {
                    UserId = g.Key,
                    Count = g.Count(),
                    FullName = g.First().User!.FullName
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<TopUserDto>> GetTopOwnersAsync(CancellationToken ct)
        {
            return await _context.Apartments
                .GroupBy(a => a.OwnerId)
                .Select(g => new TopUserDto
                {
                    UserId = g.Key,
                    Count = g.Count(),
                    FullName = g.First().Owner!.FullName
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<MonthlyStatsDto>> GetMonthlyReportsAsync(CancellationToken ct)
        {
            return await _context.Reports
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new MonthlyStatsDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync(ct);
        }

        public async Task<int> GetTodayReportsAsync(CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Reports
                .CountAsync(r => r.CreatedAt.Date == today, ct);
        }
    }
}

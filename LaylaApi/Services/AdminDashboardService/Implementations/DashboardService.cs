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

        public async Task<OverviewDto> GetOverviewAsync()
        {
            var today = DateTime.UtcNow.Date;

            return new OverviewDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalApartments = await _context.Apartments.CountAsync(),
                TotalBookings = await _context.Bookings.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalReports = await _context.Reports.CountAsync(),
                TotalRevenue = await _context.Payments
                    .Where(p => p.Status == "Completed")
                    .SumAsync(p => p.Amount),

                NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt.Date == today),
                NewApartmentsToday = await _context.Apartments.CountAsync(a => a.CreatedAt.Date == today),
                NewBookingsToday = await _context.Bookings.CountAsync(b => b.CreatedAt.Date == today),
                NewReportsToday = await _context.Reports.CountAsync(r => r.CreatedAt.Date == today)
            };
        }

        public async Task<IEnumerable<StatusStatsDto>> GetBookingStatusStatsAsync()
        {
            return await _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new StatusStatsDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyStatsDto>> GetMonthlyBookingsAsync()
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
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
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
                .ToListAsync();
        }

        public async Task<IEnumerable<TopApartmentDto>> GetTopBookedApartmentsAsync()
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
                .ToListAsync();
        }

        public async Task<IEnumerable<TopRatedApartmentDto>> GetTopRatedApartmentsAsync()
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
                .ToListAsync();
        }

        public async Task<IEnumerable<TopUserDto>> GetTopRentersAsync()
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
                .ToListAsync();
        }

        public async Task<IEnumerable<TopUserDto>> GetTopOwnersAsync()
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
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyStatsDto>> GetMonthlyReportsAsync()
        {
            return await _context.Reports
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new MonthlyStatsDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<int> GetTodayReportsAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Reports
                .CountAsync(r => r.CreatedAt.Date == today);
        }
    }
}

using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ReportService : IReportService
    {
        private readonly LaylaContext _context;

        public ReportService(LaylaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.Apartment)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Apartment)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Report>> GetByApartmentIdAsync(int apartmentId)
        {
            return await _context.Reports
                .Where(r => r.ApartmentId == apartmentId)
                .Include(r => r.Reporter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByReporterIdAsync(int userId)
        {
            return await _context.Reports
                .Where(r => r.ReporterId == userId)
                .Include(r => r.Apartment)
                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(int reporterId, int apartmentId)
        {
            return await _context.Reports
                .AsNoTracking()
                .AnyAsync(r => r.ReporterId == reporterId && r.ApartmentId == apartmentId);
        }

        public async Task<Report> AddAsync(Report report)
        {
           
            var apartment = await _context.Apartments.FirstOrDefaultAsync(a => a.Id == report.ApartmentId);
            if (apartment == null)
                throw new KeyNotFoundException("Apartment does not exist");

            var reporter = await _context.Users.FirstOrDefaultAsync(u => u.Id == report.ReporterId);
            if (reporter == null)
                throw new KeyNotFoundException("reporter does not exist");

            report = Report.Create(apartment, reporter, report.Reason);

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<Report?> UpdateStatusAsync(int id, string newStatus)
        {
            var existing = await _context.Reports.FindAsync(id);
            if (existing == null) return null;

            existing.Status = newStatus;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Reports.FindAsync(id);
            if (existing == null) return false;

            _context.Reports.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

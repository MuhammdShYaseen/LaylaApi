using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using static LaylaApi.Models.MainModels.Report;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ReportService : IReportService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;

        public ReportService(LaylaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReportDto>> GetAllAsync()
        {
            var reports = await _context.Reports
                 .AsNoTracking()
                  .OrderByDescending(r => r.CreatedAt)
                   .ToListAsync();

            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }

        public async Task<ReportDto> GetByIdAsync(int reportId, int userId, bool isAdmin)
        {
            var report = await _context.Reports.AsNoTracking().FirstOrDefaultAsync(r => r.Id == reportId)?? 
                throw new KeyNotFoundException("Report not found.");

            if (!HasAccess(report, userId, isAdmin))
                throw new UnauthorizedAccessException("Access denied.");

            return _mapper.Map<ReportDto>(report);
        }

        private bool HasAccess(Report report, int userId, bool isAdmin)
        {
            return isAdmin || report.ReporterId == userId;
        }

        public async Task<IEnumerable<ReportDto>> GetByApartmentIdAsync(int apartmentId)
        {
            var reports = await _context.Reports
                 .AsNoTracking()
                 .Where(r => r.ApartmentId == apartmentId)
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();

            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }

        public async Task<IEnumerable<ReportDto>> GetByReporterIdAsync(int userId)
        {
            var reports = await _context.Reports
                .AsNoTracking()
                .Where(r => r.ReporterId == userId)
                .Include(r => r.Apartment)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReportDto>>(reports);
        }
        public async Task<bool> ExistsAsync(int reporterId, int apartmentId)
        {
            return await _context.Reports
                .AsNoTracking()
                .AnyAsync(r => r.ReporterId == reporterId && r.ApartmentId == apartmentId);
        }

        public async Task<ReportDto> CreateAsync(ReportCreateDto model, int userId, bool isAdmin)
        {
            if (model.ApartmentId <= 0)
                throw new BadHttpRequestException("ApartmentId is required.");

            // تحقق سريع: وجود الشقة + OwnerId فقط
            var apartmentOwnerId = await _context.Apartments
                .AsNoTracking()
                .Where(a => a.Id == model.ApartmentId)
                .Select(a => a.OwnerId)
                .SingleOrDefaultAsync();

            if (apartmentOwnerId == default)
                throw new KeyNotFoundException("Apartment not found.");

            // منع التبليغ عن شقته
            if (apartmentOwnerId == userId)
                throw new BadHttpRequestException("You cannot report your own apartment.");

            // منع التبليغ المكرر
            var alreadyReported = await _context.Reports
                .AnyAsync(r =>  r.ApartmentId == model.ApartmentId && r.ReporterId == userId);

            if (alreadyReported)
                throw new BadHttpRequestException("You have already reported this apartment.");

            // إنشاء الكيان من الـ Domain
            var report = Report.Create(model.ApartmentId, userId, model.Reason);

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReportDto>(report);
        }

        public async Task<ReportDto> UpdateStatusAsync(int id, ReportStatus newStatus)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id)?? 
                throw new KeyNotFoundException("Report not found.");

            if (report.Status == newStatus)
                return _mapper.Map<ReportDto>(report); // no-op but safe

            report.ChangeStatus(newStatus);

            await _context.SaveChangesAsync();

            return _mapper.Map<ReportDto>(report);
        }

        public async Task DeleteAsync(int reportId, int userId, bool isAdmin)
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId)
                ?? throw new KeyNotFoundException("Report not found.");

            if (!HasAccess(report, userId, isAdmin))
                throw new UnauthorizedAccessException("Access denied.");

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
        }
    }
}

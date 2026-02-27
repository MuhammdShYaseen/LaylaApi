using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using static LaylaApi.Models.MainModels.Report;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDto>> GetAllAsync(CancellationToken ct);
        Task<ReportDto> GetByIdAsync(int reportId, int userId, bool isAdmin, CancellationToken ct);
        Task<bool> ExistsAsync(int reporterId, int apartmentId, CancellationToken ct);
        Task<IEnumerable<ReportDto>> GetByApartmentIdAsync(int apartmentId, CancellationToken ct);
        Task<IEnumerable<ReportDto>> GetByReporterIdAsync(int userId, CancellationToken ct);
        Task<ReportDto> CreateAsync(ReportCreateDto model, int userId, bool isAdmin, CancellationToken ct);
        Task<ReportDto> UpdateStatusAsync(int id, ReportStatus newStatus, CancellationToken ct);
        Task DeleteAsync(int reportId, int userId, bool isAdmin, CancellationToken ct);
    }
}

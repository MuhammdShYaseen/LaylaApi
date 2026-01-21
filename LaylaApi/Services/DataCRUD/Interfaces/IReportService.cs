using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using static LaylaApi.Models.MainModels.Report;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDto>> GetAllAsync();
        Task<ReportDto> GetByIdAsync(int reportId, int userId, bool isAdmin);
        Task<bool> ExistsAsync(int reporterId, int apartmentId);
        Task<IEnumerable<ReportDto>> GetByApartmentIdAsync(int apartmentId);
        Task<IEnumerable<ReportDto>> GetByReporterIdAsync(int userId);
        Task<ReportDto> CreateAsync(ReportCreateDto model, int userId, bool isAdmin);
        Task<ReportDto> UpdateStatusAsync(int id, ReportStatus newStatus);
        Task DeleteAsync(int reportId, int userId, bool isAdmin);
    }
}

using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int reporterId, int apartmentId);
        Task<IEnumerable<Report>> GetByApartmentIdAsync(int apartmentId);
        Task<IEnumerable<Report>> GetByReporterIdAsync(int userId);
        Task<Report> AddAsync(Report report);
        Task<Report?> UpdateStatusAsync(int id, string newStatus);
        Task<bool> DeleteAsync(int id);
    }
}

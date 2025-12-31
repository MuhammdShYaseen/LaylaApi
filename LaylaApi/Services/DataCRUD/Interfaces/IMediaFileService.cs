using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IMediaFileService
    {
        Task<IEnumerable<MediaFile>> GetByApartmentIdAsync(int apartmentId);
        Task<MediaFile?> GetByIdAsync(int id);
        Task<List<MediaFile>> UploadFilesAsync(int apartmentId, List<IFormFile> files, string rootPath);
        Task<bool> DeleteAsync(int id, string rootPath);
        
    }
}

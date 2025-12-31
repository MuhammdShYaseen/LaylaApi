using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class MediaFileService :IMediaFileService
    {
        private readonly LaylaContext _context;

        public MediaFileService(LaylaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaFile>> GetByApartmentIdAsync(int apartmentId)
        {
            return await _context.MediaFiles
                .Where(f => f.ApartmentId == apartmentId)
                .ToListAsync();
        }

        public async Task<MediaFile?> GetByIdAsync(int id)
        {
            return await _context.MediaFiles.FindAsync(id);
        }


        public async Task<List<MediaFile>> UploadFilesAsync(int apartmentId, List<IFormFile> files, string rootPath)
        {
            var results = new List<MediaFile>();

            string folderPath = Path.Combine(rootPath, "uploads", "apartments", apartmentId.ToString());

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            foreach (var file in files)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();

                // للتعرف على نوع الملف
                string fileType = (extension == ".mp4" || extension == ".mov" || extension == ".avi") ? "video" : "image";

                string fileName = $"{Guid.NewGuid()}{extension}";
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var media = new MediaFile
                {
                    ApartmentId = apartmentId,
                    FileUrl = $"/uploads/apartments/{apartmentId}/{fileName}",
                    FileType = fileType
                };

                _context.MediaFiles.Add(media);
                results.Add(media);
            }

            await _context.SaveChangesAsync();
            return results;
        }
        public async Task<bool> DeleteAsync(int id, string rootPath)
        {
            var media = await _context.MediaFiles.FindAsync(id);
            if (media == null) return false;

            string fullPath = Path.Combine(rootPath, media.FileUrl.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.MediaFiles.Remove(media);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

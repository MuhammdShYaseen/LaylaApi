using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class MediaFileService :IMediaFileService
    {
        private readonly LaylaContext _context;
        private readonly IApartmentService _ApartmentService;
        private static readonly HashSet<string> AllowedExtensions =[".jpg", ".jpeg", ".png", ".webp",".mp4", ".mov"];
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _Mapper;
        public MediaFileService(LaylaContext context, IApartmentService apartmentService, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _ApartmentService = apartmentService;
            _env = env;
            _Mapper = mapper;
        }

        public async Task<IEnumerable<MediaFileDto>> GetByApartmentIdAsync(int apartmentId)
        {
            var result = await _context.MediaFiles.Where(f => f.ApartmentId == apartmentId).ToListAsync();
            return _Mapper.Map<IEnumerable<MediaFileDto>>(result);
        }

        public async Task<MediaFile?> GetByIdAsync(int id)
        {
            return await _context.MediaFiles.FindAsync(id);
        }


        public async Task<List<MediaFile>> UploadFilesAsync(int apartmentId, List<IFormFile> files, int userId, bool isAdmin)
        {
            if (files == null || files.Count == 0)
                throw new BadHttpRequestException("No files received.");

            var apartment = await _ApartmentService.GetEntityByIdAsync(apartmentId)
                ?? throw new KeyNotFoundException("Apartment not found.");

            if (!HasApartmentFilesAccess(apartment, userId, isAdmin))
                throw new UnauthorizedAccessException();

            ValidateFiles(files);

            var uploadRoot = Path.Combine(_env.WebRootPath, "uploads", "apartments", apartmentId.ToString());
            Directory.CreateDirectory(uploadRoot);

            var results = new List<MediaFile>();

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(uploadRoot, fileName);

                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                string mediaUrl = $"/uploads/apartments/{apartmentId}/{fileName}";

                string fileType = IsVideo(ext) ? "video" : "image";

                var media = MediaFile.Create(apartmentId, mediaUrl, fileType);

                results.Add(media);
            }

            _context.MediaFiles.AddRange(results);
            await _context.SaveChangesAsync();

            return results;
        }

        private bool HasApartmentFilesAccess(Apartment apartment, int UserId, bool isAdmin)
        {
            var userId = UserId;
            return apartment.OwnerId == userId || isAdmin;
        }

        private void ValidateFiles(IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length == 0)
                    throw new BadHttpRequestException("Empty file.");

                if (file.Length > 20_000_000)
                    throw new BadHttpRequestException("File size exceeds limit.");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                    throw new BadHttpRequestException($"File type {ext} is not allowed.");
            }
        }

        private static bool IsVideo(string ext) => ext is ".mp4" or ".mov";

        public async Task<bool> DeleteAsync(int mediaId, int userId, bool isAdmin)
        {
            var media = await _context.MediaFiles.Include(m => m.Apartment).FirstOrDefaultAsync(m => m.Id == mediaId);

            if (media == null)
                throw new KeyNotFoundException("Media file not found.");

            if (media.Apartment == null)
                throw new InvalidOperationException("Media is not linked to an apartment.");

            if (!HasApartmentFilesAccess(media.Apartment, userId, isAdmin))
                throw new UnauthorizedAccessException("You are not allowed to delete this file.");

            DeletePhysicalFile(media.FileUrl);

            _context.MediaFiles.Remove(media);
            await _context.SaveChangesAsync();

            return true;
        }
        private void DeletePhysicalFile(string fileUrl)
        {
            var fullPath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}

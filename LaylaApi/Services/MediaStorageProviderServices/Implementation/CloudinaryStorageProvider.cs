using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LaylaApi.DataRepository;
using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.MediaStorageProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using static LaylaApi.Models.MainModels.MediaFile;

namespace LaylaApi.Services.MediaStorageProviderServices.Implementation
{
    public class CloudinaryStorageProvider : IStorageProvider
    {
        private readonly Cloudinary _cloudinary;
        private readonly IRepository<MediaFile> _repository;
        private static readonly string[] AllowedFormats = {"jpg", "png", "webp", "mp4"};
        public CloudinaryStorageProvider(Cloudinary cloudinary, IRepository<MediaFile> repository)
        {
            _cloudinary = cloudinary;
            _repository = repository;
        }
        public async Task<UploadSignatureDto> CreateUploadSignatureAsync(int userId, int apartmentId)
        {
            var used = await _repository.Query(true)
            .Where(x => x.UserId == userId && x.Status == MediaStatus.Approved)
            .SumAsync(x => (long?)x.Bytes) ?? 0;

            if (used > 2L * 1024 * 1024 * 1024) // 2GB
                throw new InvalidOperationException("Quota exceeded");

            var media =  MediaFile.CreatePending(userId, apartmentId, "Cloudinary");

            await _repository.AddAsync(media);
            await _repository.SaveChangesAsync();

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var folder = $"users/{userId}/apartments/{apartmentId}";

            var parameters = new SortedDictionary<string, object>
        {
            { "timestamp", timestamp },
            { "folder", folder },
            { "resource_type", "auto" },
            { "allowed_formats", "jpg,png,webp,mp4" },
            { "max_file_size", 50_000_000 },
            { "context", $"media_id={media.Id}" }
        };

            var signature = _cloudinary.Api.SignParameters(parameters);

            return new UploadSignatureDto
            {
                Signature = signature,
                Timestamp = timestamp,
                ApiKey = _cloudinary.Api.Account.ApiKey,
                CloudName = _cloudinary.Api.Account.Cloud,
                Folder = folder,
                MaxFileSize = 50_000_000,
                MediaId = media.Id
            };
        }

        public async Task<bool> DeleteAsync(int mediaId)
        {
            var media = await _repository.GetByIdAsync(mediaId);

            if (media == null)
                return false;

            if (!string.IsNullOrEmpty(media.PublicId))
            {
                await _cloudinary.DestroyAsync(new DeletionParams(media.PublicId)
                {
                    ResourceType =
                        media.FileType == "video" ? ResourceType.Video : ResourceType.Image
                });
            }

            media.ChangeMediaStatus(MediaStatus.Deleted);
            media.Delete();
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task HandleWebhookAsync(WebhookDto data)
        {
            if (!int.TryParse(data.Context?["media_id"], out var mediaId))
                return;

            var media = await _repository.GetByIdAsync(mediaId);

            if (media == null)
                return;

            if (string.IsNullOrEmpty(data.PublicId) || string.IsNullOrEmpty(data.ResourceType))
            {
                media.ChangeMediaStatus(MediaStatus.Rejected);
                await _repository.SaveChangesAsync();
                return;
            }
            // Validation
            if (data.Bytes > 50_000_000 ||!AllowedFormats.Contains(data.Format!.ToLowerInvariant()))
            {
                await DeleteFromCloudinary(data.PublicId!, data.ResourceType!);
                media.ChangeMediaStatus(MediaStatus.Rejected);
            }
            else
            {
                media.UpdateToApproved(data.PublicId!, data.SecureUrl!,
                                       data.Format!, data.Bytes,
                                       data.Width ?? 0, data.Height ?? 0,
                                       data.Duration ?? 0, data.ResourceType!);
            }

            await _repository.SaveChangesAsync();
        }
        private async Task DeleteFromCloudinary(string publicId, string resourceType)
        {
            var type = resourceType == "video"
                ? ResourceType.Video
                : ResourceType.Image;

            await _cloudinary.DestroyAsync(new DeletionParams(publicId)
            {
                ResourceType = type
            });
        }
    }
}

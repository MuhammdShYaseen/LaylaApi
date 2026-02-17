using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LaylaApi.DataRepository;
using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Services.MediaStorageProviderServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static LaylaApi.Models.MainModels.MediaFile;

namespace LaylaApi.Services.MediaStorageProviderServices.Implementation
{
    public class CloudinaryStorageProvider : IStorageProvider
    {
        private readonly Cloudinary _cloudinary;
        private readonly IRepository<MediaFile> _repository;
        private readonly IApartmentService _apartmentService;
        private static readonly string[] AllowedFormats = {"jpg", "png", "webp", "mp4"};
        public enum WebhookResult
        {
            Success,
            Unauthorized,
            Invalid
        }
        public CloudinaryStorageProvider(Cloudinary cloudinary, IRepository<MediaFile> repository, IApartmentService apartmentService)
        {
            _cloudinary = cloudinary;
            _repository = repository;
            _apartmentService = apartmentService;
        }
     
        public async Task<UploadSignatureDto> CreateUploadSignatureAsync(int userId, int apartmentId, bool isAdmin, CancellationToken ct)
        {

            await HasPermission(userId, apartmentId, isAdmin);

            var used = await _repository.Query()
                          .Where(x => x.UserId == userId && x.Status == MediaStatus.Approved)
                          .SumAsync(x => (long?)x.Bytes, ct) ?? 0;

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

        public async Task<WebhookResult> ProcessWebhookAsync(HttpRequest request, CancellationToken ct)
        {
            // 1️⃣ قراءة الـ body
            string body;
            using (var reader = new StreamReader(request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            // 2️⃣ الحصول على Headers
            var timestamp = request.Headers["X-Cld-Timestamp"].FirstOrDefault();
            var signature = request.Headers["X-Cld-Signature"].FirstOrDefault();

            // 3️⃣ تحقق من وجود البيانات
            if (string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(signature))
                return WebhookResult.Unauthorized;

            // 4️⃣ تحقق من التوقيع
            if (!IsValidSignature(body, timestamp, signature))
                return WebhookResult.Unauthorized;

            // 5️⃣ تحويل body إلى DTO
            var dto = JsonSerializer.Deserialize<WebhookDto>(body);

            if (dto == null)
                return WebhookResult.Invalid;

            // 6️⃣ معالجة المنطق
            await HandleWebhookAsync(dto, ct);

            return WebhookResult.Success;
        }

        public async Task<bool> DeleteAsync(int mediaId, int CurrentUserId, bool isAdmin, CancellationToken ct)
        {
            var media = await _repository.GetByIdAsync(mediaId, ct);

            if (media == null)
                return false;

            if (isAdmin == false && media.UserId != CurrentUserId)
                throw new UnauthorizedAccessException("you do not have a permission to delete this media");

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

        #region Helpper
        private async Task HasPermission(int userId, int apartmentId, bool isAdmin)
        {
            var apartment = await _apartmentService.GetEntityByIdAsync(apartmentId);

            if (isAdmin == false && apartment.OwnerId != userId)
                throw new UnauthorizedAccessException("You are not own this apartment");
        }
        private async Task HandleWebhookAsync(WebhookDto data, CancellationToken ct)
        {
            if (!int.TryParse(data.Context?["media_id"], out var mediaId))
                return;

            var media = await _repository.GetByIdAsync(mediaId, ct);

            if (media == null)
                return;

            if (media.Status != MediaStatus.Pending)
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
                                       0, data.ResourceType!);
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
        private bool IsValidSignature(string body, string timestamp, string signature)
        {
            if (string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(signature))
                return false;

            if (!long.TryParse(timestamp, out var timestampLong))
                return false;

            var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestampLong);

            if (requestTime < DateTimeOffset.UtcNow.AddMinutes(-5) || requestTime > DateTimeOffset.UtcNow.AddMinutes(5))
                return false;

            var apiSecret = _cloudinary.Api.Account.ApiSecret;

            var signedPayload = body + timestamp + apiSecret;

            using var sha1 = SHA1.Create();

            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));

            var computedSignature =
                BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedSignature),
                Encoding.UTF8.GetBytes(signature)
            );
        }
        #endregion
    }
}

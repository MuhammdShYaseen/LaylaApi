using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;


namespace LaylaApi.Models.MainModels
{
    public class MediaFile : Entity
    {
        public enum MediaStatus
        {
            Pending = 0,   // Uploaded but not verified yet
            Approved = 1,
            Rejected = 2,
            Deleted = 3
        }

        [Required]
        public int ApartmentId { get; private set; }

        [ForeignKey("ApartmentId")]
        public Apartment? Apartment { get; set; }

        [Required, MaxLength(300)]
        public string FileUrl { get; private set; } = string.Empty; // رابط الصورة أو الفيديو على السيرفر

        [Required]
        public string FileType { get; private set; } = "image"; // "image" أو "video"

        
        // Owner
        public int UserId { get; private set; }
        public string PublicId { get; private set; } = "";
        public string Format { get; private set; } = "";

        // Metadata
        public long Bytes { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double Duration { get; private set; } // For video
        
        //Status
        public MediaStatus Status { get; private set; }
        public bool IsPrimary { get; private set; } = false;

        //Provider
        public string MediaStorageProvider { get; private set; } = "";

        public static MediaFile Create(int apartmentId, string fileUrl, string fileType = "image")
        {
            
            var normalizedType = fileType?.Trim().ToLower();
            if (normalizedType is not ("image" or "video"))
                throw new ArgumentException("FileType must be either 'image' or 'video'.", nameof(fileType));

            var media = new MediaFile
            {
                ApartmentId = apartmentId,
                FileUrl = fileUrl,
                FileType = normalizedType                
            };
            return media;
        }

        public void SetAsPrime()
        {
            IsPrimary = true;
        }
        public static MediaFile CreatePending(int userId, int apartmentId, string mediaStorageProvider)
        {
            return new MediaFile
            {
                UserId = userId,
                ApartmentId = apartmentId ,
                Status = MediaStatus.Pending,
                MediaStorageProvider = mediaStorageProvider
            };
        }

        public void UpdateToApproved(string publicId, string secureUrl, string format,long bytes, int width, int height, double duration,string resourceType)
        {
            PublicId = publicId;
            FileUrl = secureUrl;

            Format = format;
            Bytes =  bytes;

            Width = width;
            Height = height;
            Duration = duration;

            FileType = resourceType;
            Status = MediaStatus.Approved;
        }

        public void ChangeMediaStatus(MediaStatus newStatus)
        {
            if (Status != newStatus)
                Status = newStatus;
        }
    }
}
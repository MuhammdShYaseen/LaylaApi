using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.Models.MainModels
{
    public class MediaFile : Entity
    {

        [Required]
        public int ApartmentId { get; private set; }

        [ForeignKey("ApartmentId")]
        public Apartment? Apartment { get; set; }

        [Required, MaxLength(300)]
        public string FileUrl { get; private set; } = string.Empty; // رابط الصورة أو الفيديو على السيرفر

        [Required]
        public string FileType { get; private set; } = "image"; // "image" أو "video"

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
            //media.AddDomainEvent(new MediaUploadedEvent(media));
            return media;
        }
    }
}
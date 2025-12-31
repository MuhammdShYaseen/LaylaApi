using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class MediaUploadedEvent : IEvent
    {
        public int MediaFileId { get; }
        public int ApartmentId { get; }
        public string FileUrl { get; }
        public string FileType { get; }

        public MediaUploadedEvent(int mediaFileId, int apartmentId, string fileUrl, string fileType)
        {
            MediaFileId = mediaFileId;
            ApartmentId = apartmentId;
            FileUrl = fileUrl;
            FileType = fileType;
        }
    }
}

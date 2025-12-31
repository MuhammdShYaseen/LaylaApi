using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class MediaFileCreateDto
    {
        [Required]
        public int ApartmentId { get; set; }

        [Required, MaxLength(300)]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        public string FileType { get; set; } = "image";
    }
}

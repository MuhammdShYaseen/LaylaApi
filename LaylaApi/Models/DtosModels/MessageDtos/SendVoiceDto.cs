using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.DtosModels.MessageDtos
{
    public sealed class SendVoiceDto
    {
        public int ApartmentId { get; init; }

        [Required]
        public IFormFile AudioFile { get; init; } = default!;

        [Range(1, 600)]
        public int DurationSeconds { get; init; }
    }
}

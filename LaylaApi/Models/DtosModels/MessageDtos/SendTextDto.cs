using LaylaApi.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.DtosModels.MessageDtos
{
    public sealed class SendTextDto
    {
        [Required]
        public int ApartmentId { get; init; }

        [Required]
        [StringLength(1500, MinimumLength = 1)]
        [NotWhiteSpace]
        public string Content { get; init; } = string.Empty;
    }
}

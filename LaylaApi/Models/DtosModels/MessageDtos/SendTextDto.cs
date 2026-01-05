using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.DtosModels.MessageDtos
{
    public sealed class SendTextDto
    {
        public int ApartmentId { get; init; }

        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string Content { get; init; } = string.Empty;
    }
}

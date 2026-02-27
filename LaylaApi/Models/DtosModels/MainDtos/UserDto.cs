namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; } = default!;

        public string Role { get; set; } = default!;

        public string Language { get; set; } = default!;

        public int ApartmentsCount { get; set; }

        public int BookingsCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

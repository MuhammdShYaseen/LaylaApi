namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public string Lang { get; set; } = default!;
    }
}

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class DeviceTokenUpsertDto
    {
        public string? Token { get; set; }
        public string? Platform { get; set; } 
        public string? DeviceId { get; private set; }
    }
}

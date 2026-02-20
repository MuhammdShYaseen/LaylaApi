using LaylaApi.Models.DtosModels.ExternalServicesDtos;

namespace LaylaApi.Services.LocationFromIPService.Interfaces
{
    public interface ILocationFromIPExternalService
    {
        Task <IpApiResponseDto?> GetAsync (string ip, CancellationToken ct = default);
    }
}

using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using static LaylaApi.Services.DataCRUD.Implementations.ContractService;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllAsync(CancellationToken ct);
        Task<ContractDto> GetByIdAsync(int id, int userId, bool isAdmin, CancellationToken ct);
        Task<Contract> GetEntityByIdAsync(int id);
        Task<Contract> AddEntityAsync(int bookingId, string specialTerms);
        Task<ContractDto> UpdateAsync(int id, CreateContractDto dto, CancellationToken ct);
        Task<ContractDto> UpdateEntityAsync(Contract contract);
        Task<ContractDto> SignContractAsync(int Id, int userId, bool isAdmin, ContractSigner contractSigner, CancellationToken ct);
        Task<ContractDto> GenerateContractAsync(int userId, ContractCreateDto model, bool isAdmin, CancellationToken ct);
        Task<bool> DeleteAsync(int id, int userId, bool isAdmin, CancellationToken ct);
        Task<ContractDto> GetByBookingIdAsync(int bookingId,int userId,bool isAdmin, CancellationToken ct);
        string GenerateContractPdf(Contract contract, Booking booking, Apartment apartment, User renter, User owner, string specialTerms);
    }
}

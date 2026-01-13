using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllAsync();
        Task<ContractDto> GetByIdAsync(int id, int userId, bool isAdmin);
        Task<Contract> GetEntityByIdAsync(int id);
        Task<ContractDto> AddAsync(CreateContractDto dto);
        Task<Contract> AddEntityAsync(int bookingId, string specialTerms);
        Task<ContractDto> UpdateAsync(int id, CreateContractDto dto);
        Task<ContractDto> UpdateEntityAsync(Contract contract);
        Task<ContractDto> SignContractAsync(int Id, int userId, bool isAdmin);
        Task<bool> DeleteAsync(int id);
        Task<ContractDto> GetByBookingIdAsync(int bookingId,int userId,bool isAdmin);
        string GenerateContractPdf(Contract contract, Booking booking, Apartment apartment, User renter, User owner, string specialTerms);
    }
}

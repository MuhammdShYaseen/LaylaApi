using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> GetCountAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<UpdateUserDto?> UpdateAsync(int id, UpdateUserDto dto, bool iSAdmin);
        Task<UpdateUserDto?> UpdateEmailAsync(int id, bool isAdmin, string newEmail);
        Task<bool> DeleteAsync(int id);
        Task<string> GetUserPreferredLanguage(int userId);
    }
}

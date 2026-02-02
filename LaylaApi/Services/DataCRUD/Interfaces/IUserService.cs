using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailTokenAsync(string emailToken);
        Task<int> GetCountAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<User?> GetByResetTokenAsync(string token);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneAsync(string phone);
        Task<UpdateUserDto?> UpdateAsync(int id, UpdateUserDto dto, bool iSAdmin);
        Task<UpdateUserDto?> UpdateEmailAsync(int targetUserId, int currentUserId, bool isAdmin, string newEmail);
        Task<bool> DeleteAsync(int id);
        Task<string> GetUserPreferredLanguage(int userId);
        Task SaveAsync();
    }
}

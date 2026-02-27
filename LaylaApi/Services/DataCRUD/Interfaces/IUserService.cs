using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct);
        Task<User?> GetByEmailTokenAsync(string emailToken, CancellationToken ct);
        Task<int> GetCountAsync(CancellationToken ct);
        Task<User?> GetByIdAsync(int id, CancellationToken ct);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User> AddAsync(User user, CancellationToken ct);
        Task<User?> GetByResetTokenAsync(string token, CancellationToken ct);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
        Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct);
        Task<UpdateUserDto?> UpdateAsync(int id, UpdateUserDto dto, bool iSAdmin, CancellationToken ct);
        Task<UpdateUserDto?> UpdateEmailAsync(int targetUserId, int currentUserId, bool isAdmin, string newEmail, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<string> GetUserPreferredLanguage(int userId, CancellationToken ct);
        Task SaveAsync();
    }
}

using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class UserService : IUserService
    {
        private readonly LaylaContext _context;

        public UserService(LaylaContext context)
        {
            _context = context;
        }

        public async Task<int>GetCountAsync()=>
            await _context.Users.CountAsync();
        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return null;

            existing.Update(user.FullName, user.Email, user.PhoneNumber, user.Lang);

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return false;

            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetUserPreferredLanguage(int userId)
        {
            var existing = await _context.Users.FindAsync(userId);
            if (existing == null) throw new DirectoryNotFoundException("UserNotFound");
            return existing.Lang;

        }
    }
}

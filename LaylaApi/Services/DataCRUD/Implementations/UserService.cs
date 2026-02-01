using AutoMapper;
using Azure.Core;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Services.LanguageServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class UserService : IUserService
    {
        private readonly LaylaContext _context;
        private readonly ISupportedLanguagePolicy _languagePolicy;
        private readonly IMapper _mapper;

        public UserService(LaylaContext context, ISupportedLanguagePolicy languagePolicy, IMapper mapper)
        {
            _context = context;
            _languagePolicy = languagePolicy;
            _mapper = mapper;
        }

        public async Task<int>GetCountAsync()=>
            await _context.Users.CountAsync();
        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email!.Value.ToLower() == email.ToLower());

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UpdateUserDto?> UpdateEmailAsync(int id, bool isAdmin, string newEmail)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return null;

            // صلاحيات
            if (!isAdmin && user.Id != id)
                throw new UnauthorizedAccessException("Access denied.");

            // Normalize email
            newEmail = newEmail.Trim().ToLowerInvariant();

            // لا تعيد الطلب إذا نفس الإيميل
            if (user.Email!.Value == newEmail)
                return _mapper.Map<UpdateUserDto>(user);

            // تحقق من التكرار
            var exists = await _context.Users
                .AnyAsync(u =>
                    u.Email!.Value == newEmail &&
                    u.Id != id);

            if (exists)
                throw new ArgumentException("Email is already in use.");

            // Domain logic
            user.RequestEmailChange(newEmail);

            await _context.SaveChangesAsync();

            return _mapper.Map<UpdateUserDto>(user);

        }

        public async Task<UpdateUserDto?> UpdateAsync(int id, UpdateUserDto dto, bool isAdmin)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return null;

            if (!isAdmin && user.Id != id)
                throw new UnauthorizedAccessException("Access denied.");

            // Delegate logic to Aggregate
            user.Update(dto.FullName, dto.PhoneNumber, dto.Lang, _languagePolicy);

            await _context.SaveChangesAsync();

            return _mapper.Map<UpdateUserDto>(user);
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
            return existing.Lang!.Code;

        }

        public async Task<User?> GetByResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetPasswordToken == token &&
                u.ResetPasswordTokenExpires > DateTime.UtcNow);
        }

        public async Task<bool> ExistsByEmailAsync(string email)=>
             await _context.Users.AnyAsync(u => u.Email!.Value.ToLower() == email.ToLower());
        

        public async Task<bool> ExistsByPhoneAsync(string phone)=>
             await _context.Users.AnyAsync(u => u.PhoneNumber!.Value == phone);

        public async Task<User?> GetByEmailTokenAsync(string emailToken)=>
            await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == emailToken);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

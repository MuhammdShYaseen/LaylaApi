using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Services.LanguageServices;
using LaylaApi.ValueObjects.UserValueObject;
using Microsoft.EntityFrameworkCore;


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

        public async Task<int>GetCountAsync(CancellationToken ct)=>
            await _context.Users.CountAsync(ct);
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct) =>
            await _context.Users.ToListAsync(ct);

        public async Task<User?> GetByIdAsync(int id, CancellationToken ct) =>
            await _context.Users.FindAsync(id, ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == Email.Create(normalized), ct);
        }

        public async Task<User> AddAsync(User user, CancellationToken ct)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UpdateUserDto?> UpdateEmailAsync(int targetUserId, int currentUserId, bool isAdmin, string newEmail, CancellationToken ct)
        {
            var user = await _context.Users.FindAsync(targetUserId, ct);

            if (user == null)
                return null;

            // صلاحيات
            if (!isAdmin && currentUserId != targetUserId)
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
                    u.Id != currentUserId, ct);

            if (exists)
                throw new ArgumentException("Email is already in use.");

            // Domain logic
            user.RequestEmailChange(newEmail);

            await _context.SaveChangesAsync();

            return _mapper.Map<UpdateUserDto>(user);

        }

        public async Task<UpdateUserDto?> UpdateAsync(int id, UpdateUserDto dto, bool isAdmin, CancellationToken ct)
        {
            var user = await _context.Users.FindAsync(id, ct);

            if (user == null)
                return null;

            if (!isAdmin && user.Id != id)
                throw new UnauthorizedAccessException("Access denied.");

            // Delegate logic to Aggregate
            user.Update(dto.FullName, dto.PhoneNumber, dto.Lang, _languagePolicy);

            await _context.SaveChangesAsync();

            return _mapper.Map<UpdateUserDto>(user);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var existing = await _context.Users.FindAsync(id, ct);
            if (existing == null) return false;

            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetUserPreferredLanguage(int userId, CancellationToken ct)
        {
            var existing = await _context.Users.FindAsync(userId,ct);
            if (existing == null) throw new DirectoryNotFoundException("UserNotFound");
            return existing.Lang!.Code;

        }

        public async Task<User?> GetByResetTokenAsync(string token, CancellationToken ct)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetPasswordToken == token &&
                u.ResetPasswordTokenExpires > DateTime.UtcNow, ct);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
             await _context.Users.AnyAsync(u => u.Email! == Email.Create(email.Trim().ToLowerInvariant()),ct);

            
        

        public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct)=>
             await _context.Users.AnyAsync(u => u.PhoneNumber == PhoneNumber.Create(phone.Trim().ToLowerInvariant()), ct);

        public async Task<User?> GetByEmailTokenAsync(string emailToken, CancellationToken ct)=>
            await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == emailToken, ct);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

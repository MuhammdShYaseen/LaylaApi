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
            await _context.Users.AsNoTracking().CountAsync(ct);
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct) =>
            await _context.Users.AsNoTracking().ToListAsync(ct);

        public async Task<User?> GetByIdAsync(int id, CancellationToken ct) =>
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

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

        public async Task<UserDto?> UpdateEmailAsync(int targetUserId, int currentUserId, bool isAdmin, string newEmail, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetUserId, ct);

            if (user == null)
                return null;

            // صلاحيات
            if (!isAdmin && currentUserId != targetUserId)
                throw new UnauthorizedAccessException("Access denied.");

            // Normalize email
            newEmail = newEmail.Trim().ToLowerInvariant();

            // لا تعيد الطلب إذا نفس الإيميل
            if (user.Email!.Value == newEmail)
                return _mapper.Map<UserDto>(user);

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

            return _mapper.Map<UserDto>(user);

        }

        public async Task<UserDto?> UpdateAsync(int targetUserId, int currentUserId, UpdateUserDto dto, bool isAdmin, CancellationToken ct)
        {
            if (!isAdmin && currentUserId != targetUserId)
                throw new UnauthorizedAccessException("Access denied");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == targetUserId, ct);

            if (user is null)
                return null;

            user.Update(dto.FullName, dto.PhoneNumber, dto.Lang, _languagePolicy);

            await _context.SaveChangesAsync(ct);

            return _mapper.Map<UserDto>(user);
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
            var result = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    Exists = true,
                    Lang = u.Lang != null ? u.Lang.Code : null
                })
                .FirstOrDefaultAsync(ct);

            if (result is null)
                throw new KeyNotFoundException("User not found");

            if (result.Lang is null)
                return "en";

            return result.Lang;

        }

        public async Task<User?> GetByResetTokenAsync(string token, CancellationToken ct)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == token && u.ResetPasswordTokenExpires > DateTime.UtcNow, ct);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
             await _context.Users.AsNoTracking().AnyAsync(u => u.Email! == Email.Create(email.Trim().ToLowerInvariant()),ct);

            
        

        public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct)=>
             await _context.Users.AnyAsync(u => u.PhoneNumber == PhoneNumber.Create(phone.Trim().ToLowerInvariant()), ct);

        public async Task<User?> GetByEmailTokenAsync(string emailToken, CancellationToken ct)=>
            await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == emailToken, ct);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<UserDto?> GetCurrentUserAsync(int id, CancellationToken ct)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email != null ? u.Email.Value : string.Empty,
                    PhoneNumber = u.PhoneNumber != null ? u.PhoneNumber.Value : string.Empty,
                    EmailConfirmed = u.EmailConfirmed,
                    Role = u.Role,
                    Language = u.Lang != null ? u.Lang.Code : "en",
                    ApartmentsCount = u.Apartments != null ? u.Apartments.Count : 0,
                    BookingsCount = u.Bookings != null ? u.Bookings.Count : 0,
                    CreatedAt = u.CreatedAt,
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}

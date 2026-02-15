
using LaylaApi.DataRepository;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly IRepository<DeviceToken> _repository;
        public DeviceTokenService(IRepository<DeviceToken> repository) 
        {
            _repository = repository;
        }
        public async Task CleanupInactiveAsync(TimeSpan maxAge)
        {
            var cutoff = DateTime.UtcNow - maxAge;

            var oldTokens = await _repository.Query()
                .Where(dt => dt.LastSeenAt < cutoff)
                .ToListAsync();

            if (oldTokens.Any())
            {
                _repository.RemoveRange(oldTokens.ToArray());
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteAsync(int id, bool isAdmin)
        {
            if (isAdmin == false) 
                throw new UnauthorizedAccessException("you can not delete this file");

            var dvToken = await _repository.GetByIdAsync(id);
            if (dvToken == null) return false;

            _repository.HardDelete(dvToken);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DeviceToken>> GetByUserIdAsync(int userId)
        {
            return await _repository.Query()
            .Where(dt => dt.UserId == userId)
            .ToListAsync();
        }

        public async Task<DeviceToken> UpsertAsync(DeviceTokenUpsertDto dto, int currentUserId)
        {
            var existing = await _repository.Query()
                .FirstOrDefaultAsync(dt => dt.UserId == currentUserId && dt.DeviceId == dto.DeviceId);

            if (existing != null)
            {
                if (string.IsNullOrEmpty(dto.Token))
                    throw new BadHttpRequestException("this data can not be empty");

                existing.UpdateToken(dto.Token);

                _repository.Update(existing);

                await _repository.SaveChangesAsync();

                return existing;
            }

            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrEmpty(dto.DeviceId) || string.IsNullOrEmpty(dto.Platform))
                throw new BadHttpRequestException("this data can not be empty");

            var deviceToken = DeviceToken.Create(currentUserId, dto.Token , dto.Platform, dto.DeviceId);
            

            await _repository.AddAsync(deviceToken);
            await _repository.SaveChangesAsync();
            return deviceToken;
        }
    }
}

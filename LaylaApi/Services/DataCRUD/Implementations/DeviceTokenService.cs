
using AutoMapper;
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
        private readonly IMapper _mapper;
        public DeviceTokenService(IRepository<DeviceToken> repository, IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
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

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {

            if (id <= 0)
                throw new BadHttpRequestException("device id is required");

            var dvToken = await _repository.GetByIdAsync(id, ct);
            if (dvToken == null) return false;

            _repository.HardDelete(dvToken);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DeviceTokenDto>> GetByUserIdAsync(int userId, CancellationToken ct)
        {
            if (userId <= 0)
                throw new BadHttpRequestException("user id is required");

            return await _repository.Query().AsNoTracking()
            .Where(dt => dt.UserId == userId)
            .Select (d => new DeviceTokenDto
            {
                UserId = d.UserId,
                DeviceId = d.DeviceId,
                LastSeenAt = d.LastSeenAt,
                Platform = d.Platform,
                Token = d.Token
            })
            .ToListAsync(ct);
        }

        public async Task<DeviceTokenDto> UpsertAsync(DeviceTokenUpsertDto dto, int currentUserId, CancellationToken ct)
        {
            if (currentUserId <= 0)
                throw new BadHttpRequestException("user id is required");

            var existing = await _repository.Query()
                .FirstOrDefaultAsync(dt => dt.UserId == currentUserId && dt.DeviceId == dto.DeviceId, ct);

            if (string.IsNullOrEmpty(dto.Token))
                    throw new BadHttpRequestException("this data can not be empty");
            if (existing != null)
            {
                

                existing.UpdateToken(dto.Token);

                _repository.Update(existing);

                await _repository.SaveChangesAsync();

                return _mapper.Map<DeviceTokenDto>(existing) ;
            }

            var deviceToken = DeviceToken.Create(currentUserId, dto.Token , dto.Platform, dto.DeviceId);
            

            await _repository.AddAsync(deviceToken);
            await _repository.SaveChangesAsync();
            return _mapper.Map <DeviceTokenDto> (deviceToken);
        }
    }
}

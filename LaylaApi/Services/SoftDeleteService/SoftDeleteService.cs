using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.SoftDeleteService
{
    public class SoftDeleteService<T> : ISoftDeleteService<T> where T : Entity
    {
        private readonly IRepository<T> _repository;
        public SoftDeleteService(IRepository<T> repository) 
        {
            _repository = repository;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            entity.Delete();

            await _repository.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RestoreAsync(int id)
        {
            var entity = await _repository.Query()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            entity.Restore();

            await _repository.SaveChangesAsync();
            return true;
        }

        
    }
}

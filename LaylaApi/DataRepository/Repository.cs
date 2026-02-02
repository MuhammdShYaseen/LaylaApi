using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Services.SoftDeleteService;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LaylaApi.DataRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity 
    { 
        protected readonly LaylaContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ISoftDeleteService<TEntity> _softDeleteService;
        public Repository(LaylaContext context, ISoftDeleteService<TEntity> softDeleteService) 
        { 
            _context = context ??  throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
            _softDeleteService = softDeleteService;
        } 
        public virtual Task<TEntity?> GetByIdAsync(long id) 
            => _dbSet.FindAsync(id).AsTask();
        public virtual Task<TEntity?> GetByGuidAsync(Guid guid) 
            => _dbSet.FirstOrDefaultAsync(x => x.Guid == guid);
        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity); 
        }
        public virtual void Update(TEntity entity)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
        }

        public async Task<bool> SoftDelete(int id)
        {
           return await _softDeleteService.SoftDeleteAsync(id);
        }

        public async Task<bool> Restore(int id)
        {
           return await _softDeleteService.RestoreAsync(id);
        }
        public virtual void HardDelete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
             _dbSet.Remove(entity); 
        } 
        
        public virtual async Task<int> SaveChangesAsync() 
            => await _context.SaveChangesAsync();
        public virtual IQueryable<TEntity> Query(bool noTracking = false)
        => noTracking ? _dbSet.AsNoTracking() : _dbSet;
    } 
}

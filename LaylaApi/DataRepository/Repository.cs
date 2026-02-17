using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity 
    { 
        protected readonly LaylaContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IHttpContextAccessor _http;
        private CancellationToken Ct => _http.HttpContext?.RequestAborted ?? CancellationToken.None;
        public Repository(LaylaContext context, IHttpContextAccessor http) 
        { 
            _context = context ??  throw new ArgumentNullException(nameof(context));
             _http = http ?? throw new ArgumentNullException(nameof(http));
            _dbSet = context.Set<TEntity>();
        } 
        public virtual Task<TEntity?> GetByIdAsync(long id) 
            => _dbSet.FindAsync(id,Ct).AsTask();
        public virtual Task<TEntity?> GetByGuidAsync(Guid guid) 
            => _dbSet.FirstOrDefaultAsync(x => x.Guid == guid,Ct);
        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity, Ct); 
        }
        public virtual void Update(TEntity entity)
        {
            if (entity == null) 
                throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
        }
        public virtual void HardDelete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
             _dbSet.Remove(entity); 
        } 
        public virtual void RemoveRange(TEntity[] entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _dbSet.RemoveRange(entities);
        }
        public virtual async Task<int> SaveChangesAsync() 
            => await _context.SaveChangesAsync(Ct);
        public virtual IQueryable<TEntity> Query()
        =>  _dbSet;
    } 
}

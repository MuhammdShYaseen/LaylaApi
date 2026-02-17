using LaylaApi.DomainEvents.Domain.Common;
namespace LaylaApi.DataRepository
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> GetByIdAsync(long id, CancellationToken ct);
        Task<TEntity?> GetByGuidAsync(Guid guid, CancellationToken ct);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void HardDelete(TEntity entity);
        void RemoveRange(TEntity[] entities);
        IQueryable<TEntity> Query();
        Task<int> SaveChangesAsync(); // إضافة ضرورية
    }
}

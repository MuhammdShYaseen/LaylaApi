using LaylaApi.DomainEvents.Domain.Common;
namespace LaylaApi.DataRepository
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> GetByIdAsync(long id);
        Task<TEntity?> GetByGuidAsync(Guid guid);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void HardDelete(TEntity entity);
        IQueryable<TEntity> Query();
        Task<int> SaveChangesAsync(); // إضافة ضرورية
    }
}

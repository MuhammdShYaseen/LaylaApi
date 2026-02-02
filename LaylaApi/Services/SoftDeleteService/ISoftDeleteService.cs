using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Services.SoftDeleteService
{
    public interface ISoftDeleteService<T> where T : Entity
    {
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
    }
}

using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Services.SoftDeleteService
{
    public interface ISoftDeleteService<T> where T : Entity
    {
        Task<bool> SoftDeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id);
    }
}

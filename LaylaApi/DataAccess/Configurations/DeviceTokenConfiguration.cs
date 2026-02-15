using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Models.MainModels;
using LaylaApi.Models.NotificationsModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaylaApi.DataAccess.Configurations
{
    public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.HasIndex(d => d.Token).IsUnique();
            builder.HasIndex(d => new { d.UserId, d.DeviceId }) .IsUnique();
            builder.HasOne(d => d.User)
                   .WithMany(u => u.DeviceTokens)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

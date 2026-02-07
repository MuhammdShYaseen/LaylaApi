using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataAccess.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> entity)
        {
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshToken)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

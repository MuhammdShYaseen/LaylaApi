using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataAccess.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> entity)
        {
            entity.HasOne(r => r.Apartment)
                  .WithMany(a => a.Reviews)
                  .HasForeignKey(r => r.ApartmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(r => new
            {
                r.ApartmentId,
                r.Rating
            });
        }
    }
}

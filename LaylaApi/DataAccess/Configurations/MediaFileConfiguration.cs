using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataAccess.Configurations
{
    public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
    {
        public void Configure(EntityTypeBuilder<MediaFile> entity)
        {
            entity.HasOne(m => m.Apartment)
                  .WithMany(a => a.MediaFiles)
                  .HasForeignKey(m => m.ApartmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

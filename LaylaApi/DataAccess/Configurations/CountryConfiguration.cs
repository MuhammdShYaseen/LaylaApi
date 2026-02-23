using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataAccess.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code)
                   .IsRequired()
                   .HasMaxLength(2);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(c => c.Cities)
                   .WithOne(c => c.Country)
                   .HasForeignKey(c => c.CountryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

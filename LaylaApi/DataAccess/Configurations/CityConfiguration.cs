using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataAccess.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasOne(c => c.Country)
                   .WithMany(c => c.Cities)
                   .HasForeignKey(c => c.CountryId);

            builder.HasIndex(c => c.CountryId);
        }
    }

}

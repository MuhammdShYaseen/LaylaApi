using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;

namespace LaylaApi.DataAccess.Configurations
{
    public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
    {
        public void Configure(EntityTypeBuilder<Apartment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Value Object GeoLocation 
            builder.OwnsOne(a => a.Location, geo =>
            {
                geo.Property(g => g.Street).HasMaxLength(100);
                geo.Property(g => g.BuildingNumber).HasMaxLength(50);
                geo.Property(g => g.ApartmentNumber).HasMaxLength(50);
                geo.Property(g => g.City).HasMaxLength(100);
                geo.Property(g => g.District).HasMaxLength(100);
                geo.Property(g => g.Country).HasMaxLength(100);

                // Nested Value Object Coordinates
                geo.OwnsOne(g => g.Location, coord =>
                {
                    coord.Property(c => c.Latitude).HasColumnName("Latitude");
                    coord.Property(c => c.Longitude).HasColumnName("Longitude");
                });
            });
        }
    }
}

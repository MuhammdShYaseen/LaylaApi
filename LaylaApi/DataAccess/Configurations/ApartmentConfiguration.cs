using LaylaApi.Models.MainModels;
using LaylaApi.ValueObjects.ApartmentValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


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
                geo.Property(g => g.Location)
                   .HasColumnType("geography");
            });

            var moneyConverter = new ValueConverter<Money, decimal>(v => v.Value, v => Money.Create(v));
            builder.Property(x => x.PricePerDay).HasPrecision(18, 2).HasConversion(moneyConverter!);
            builder.Property(x => x.PricePerHour).HasPrecision(18, 2).HasConversion(moneyConverter!);

            builder.HasOne(a => a.Owner)
                  .WithMany(u => u.Apartments)
                  .HasForeignKey(a => a.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new
            {
                a.PricePerDay,
                a.PricePerHour,
                a.Area,
                a.FloorNumber,
                a.NumberOfBedRooms,
                a.NumberOfBathrooms
            });

            builder.HasIndex(a => new
            {
                a.IsAvailable,
                a.Type,
                a.Finishing
            });
        }

    }
}

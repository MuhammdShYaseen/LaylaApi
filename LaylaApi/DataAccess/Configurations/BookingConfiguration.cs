using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.DataAccess.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> entity)
        {
            entity.HasOne(b => b.Apartment)
                  .WithMany(a => a.Bookings)
                  .HasForeignKey(b => b.ApartmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // For availability / overlaps (confirmed only)
            entity.HasIndex(b => new
            {
                b.ApartmentId,
                b.StartDate,
                b.EndDate
            })
            .HasFilter($@"[Status] IN ({(int)BookingStatus.Accepted}, {(int)BookingStatus.Confirmed})");

            // For user queries
            entity.HasIndex(b => b.UserId);
        }
    }
}

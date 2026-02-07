using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using LaylaApi.Models.MainModels;

namespace LaylaApi.DataAccess.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> entity)
        {
            entity.HasOne(c => c.Booking)
                  .WithOne(b => b.Contract)
                  .HasForeignKey<Contract>(c => c.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

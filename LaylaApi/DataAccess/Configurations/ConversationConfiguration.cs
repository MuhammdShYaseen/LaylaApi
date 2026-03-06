using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LaylaApi.DataAccess.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> entity)
        {
            entity.HasIndex(c => new { c.ApartmentId, c.UserId }).IsUnique();
            entity.HasOne(c => c.Apartment)
                  .WithMany(a => a.Conversations)
                  .HasForeignKey(c => c.ApartmentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                  .WithMany(u => u.Conversations)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Messages)
                  .WithOne(m => m.Conversation)
                  .HasForeignKey(m => m.ConversationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => c.OwnerId);
        }
    }
}

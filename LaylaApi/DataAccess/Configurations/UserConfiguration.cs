using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Models.MainModels;
using LaylaApi.ValueObjects.UserValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LaylaApi.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .IsRequired();
            });

            var emailConverter = new ValueConverter<Email, string>(v => v.Value, v => Email.Create(v));
            var phoneConverter = new ValueConverter<PhoneNumber, string>(v => v.Value, v => PhoneNumber.Create(v));
            var languageConverter = new ValueConverter<Language, string>(v => v.Code, v => Language.FromPersistence(v));

            entity.Property(u => u.Email)
                  .HasConversion(emailConverter!)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.PhoneNumber)
                  .HasConversion(phoneConverter!)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.HasIndex(u => u.PhoneNumber).IsUnique();

            entity.Property(u => u.Lang)
                  .HasConversion(languageConverter!)
                  .HasMaxLength(5)
                  .IsRequired();
        }
    }
}

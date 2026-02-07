using System;
using System.Reflection;
using LaylaApi.DataAccess.Configurations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.MainModels;
using LaylaApi.Models.NotificationsModels;
using LaylaApi.ValueObjects.ApartmentValueObject;
using LaylaApi.ValueObjects.UserValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LaylaApi.DataAccess
{
    public class LaylaContext : DbContext
    {
        private readonly IEventDispatcher _dispatcher;
        public LaylaContext(DbContextOptions<LaylaContext> options, IEventDispatcher dispatcher) : base(options) 
        {
            _dispatcher = dispatcher;
        }
        // 🧩 تعريف الجداول
        public DbSet<User> Users { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<RefreshToken> RefreshTokens {  get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        private static void SetSoftDeleteFilter<T>(ModelBuilder builder)
        where T : Entity
        {
            builder.Entity<T>()
                   .HasQueryFilter(e => !e.IsDeleted);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ApartmentConfiguration());
            var moneyConverter = new ValueConverter<Money, decimal>(v => v.Value, v => Money.Create(v));
            var languageConverter = new ValueConverter<Language, string>(v => v.Code, v => Language.FromPersistence(v));
            var emailConverter = new ValueConverter<Email, string>(v => v.Value, v => Email.Create(v));
            var phoneConverter = new ValueConverter<PhoneNumber, string>(v => v.Value, v => PhoneNumber.Create(v));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(nameof(Entity.Guid))
                        .IsUnique();
                }

                if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(LaylaContext)
                        .GetMethod(nameof(SetSoftDeleteFilter),
                            BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(null, new object[] { modelBuilder });
                }
            }

            modelBuilder.Entity<User>(entity =>
            {
                // Email
                entity.Property(u => u.Email)
                      .HasConversion(emailConverter!)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                // Phone
                entity.Property(u => u.PhoneNumber)
                      .HasConversion(phoneConverter!)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(u => u.PhoneNumber)
                      .IsUnique();

                // Language
                entity.Property(u => u.Lang)
                      .HasConversion(languageConverter!)
                      .HasMaxLength(5)
                      .IsRequired();
            });

            // ✅ العلاقة: User (Owner) → Apartments (One-to-Many)
            modelBuilder.Entity<Apartment>(entity =>
            {
                // Decimal precision
                entity.Property(x => x.PricePerDay).HasPrecision(18, 2).HasConversion(moneyConverter!);
                entity.Property(x => x.PricePerHour).HasPrecision(18, 2).HasConversion(moneyConverter!);

                // Relationship: Apartment → Owner (User)
                entity.HasOne(a => a.Owner)
                      .WithMany(u => u.Apartments)
                      .HasForeignKey(a => a.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(a => new
                {
                    a.PricePerDay,
                    a.PricePerHour,
                    a.Area,
                    a.FloorNumber,
                    a.NumberOfBedRooms,
                    a.NumberOfBathrooms
                });

                // Flags / Enums
                entity.HasIndex(a => new
                {
                    a.IsAvailable,
                    a.Type,
                    a.Finishing
                })
                ;
            });

            // ✅ العلاقة: User  → RefreshToken (One-to-Many)
            modelBuilder.Entity<RefreshToken>()
                .HasOne(a => a.User)
                .WithMany(u => u.RefreshToken)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ العلاقة: Apartment → MediaFiles (One-to-Many)
            modelBuilder.Entity<MediaFile>()
                .HasOne(m => m.Apartment)
                .WithMany(a => a.MediaFiles)
                .HasForeignKey(m => m.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ العلاقة: Apartment → Bookings (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Apartment)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // ✅ العلاقة: User → Bookings (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.ApartmentId, b.Status, b.StartDate, b.EndDate });

            // ✅ العلاقة: Booking → Contract (One-to-One)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Booking)
                .WithOne(b => b.Contract)
                .HasForeignKey<Contract>(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ العلاقة: Booking → Payment (One-to-One)
            modelBuilder.Entity<Payment>(entity =>
            {
                // Decimal precision
                entity.Property(x => x.Amount).HasPrecision(18, 2);

                // One-to-one relationship: Payment ↔ Booking
                entity.HasOne(p => p.Booking)
                      .WithOne(b => b.Payment)
                      .HasForeignKey<Payment>(p => p.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ العلاقة: Apartment → Reviews (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Apartment)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ العلاقة: User → Reviews (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany()
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ العلاقة: Apartment → Reports (One-to-Many)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Apartment)
                .WithMany()
                .HasForeignKey(r => r.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            //Chat 
            modelBuilder.Entity<Conversation>(e =>
            {
                e.HasIndex(x => new { x.ApartmentId, x.UserId }).IsUnique();
            });
        }
        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var entitiesWithEvents = ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var events = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Domain events are dispatched only after successful persistence
            var result = await base.SaveChangesAsync(ct);

            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in events)
                await _dispatcher.EnqueueAsync(domainEvent, ct);

            return result;
        }
    }
}

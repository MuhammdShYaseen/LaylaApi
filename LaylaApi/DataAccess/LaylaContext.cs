using System;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.MainModels;
using LaylaApi.Models.NotificationsModels;
using LaylaApi.ValueObjects.ApartmentValueObject;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var moneyConverter = new ValueConverter<Money, decimal>(v => v.Value, v => Money.Create(v));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(nameof(Entity.Guid))
                        .IsUnique();
                }
            }

            // ✅ User - Email فريد
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

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

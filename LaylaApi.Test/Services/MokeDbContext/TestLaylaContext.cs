using LaylaApi.DataAccess;
using LaylaApi.DataAccess.Configurations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;

using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Services.MokeDbContext
{
    public class TestLaylaContext : LaylaContext
    {
        public TestLaylaContext(DbContextOptions<LaylaContext> options)
        : base(options, new FakeEventDispatcher())
        {
        }
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
            modelBuilder.Entity<Apartment>()
              .Property(a => a.View)
              .HasColumnName("ApartmentView");
            modelBuilder.Entity<Apartment>()
               .OwnsOne(a => a.Location, geo =>
               {
                   geo.Ignore(g => g.Location);
               });
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            ApplyGlobalFilters(modelBuilder);


        }

        private static void ApplyGlobalFilters(ModelBuilder modelBuilder)
        {
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
        }
    }
}

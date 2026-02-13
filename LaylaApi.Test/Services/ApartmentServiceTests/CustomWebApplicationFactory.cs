using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.LanguageServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Services.ApartmentServiceTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection _connection;
        private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1. إزالة DbContext الحقيقي
                RemoveDbContext<LaylaContext>(services);

                // 2. إزالة الـ Policies/Mocks القديمة
                RemoveService<ISupportedLanguagePolicy>(services);

                // 3. إضافة Mock للـ Policy
                services.AddSingleton(SupportedLanguagePolicy());

                // 4. إضافة SQLite In-Memory
                AddInMemorySqlite(services);

                // 5. تهيئة قاعدة البيانات
                InitializeDatabase(services);
            });
        }

        private void RemoveDbContext<T>(IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<T>));

            if (descriptor != null)
                services.Remove(descriptor);
        }

        private void RemoveService<T>(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(T));

            if (descriptor != null)
                services.Remove(descriptor);
        }

        private void AddInMemorySqlite(IServiceCollection services)
        {
            _connection = new SqliteConnection($"DataSource={_dbName};Mode=Memory;Cache=Shared");
            _connection.Open();

            services.AddDbContext<LaylaContext>(options =>
            {
                options.UseSqlite(_connection, x =>
                {
                    x.UseNetTopologySuite();
                    x.MigrationsAssembly(typeof(LaylaContext).Assembly.FullName);
                });
            });
        }

        private void InitializeDatabase(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LaylaContext>();

            // التأكد من حذف أي بيانات سابقة
            db.Database.EnsureDeleted();

            // تطبيق Migrations (وليس EnsureCreated!)
            db.Database.Migrate();

            // Seed data للاختبارات
            SeedTestData(db);
        }

        private void SeedTestData(LaylaContext context)
        {
            // بيانات ثابتة للاختبارات - وليس Random!
            var owner = User.Create(
                "Test Owner",
                "owner@test.com",
                "+1000000000",
                "Password123!",
                "hash",
                "en",
                "token",
                SupportedLanguagePolicy());

            context.Users.Add(owner);
            context.SaveChanges();

            // شقق ببيانات محددة للاختبار
            var apartments = new List<Apartment>
        {
            Apartment.Create(new CreateApartmentDto
            {
                Title = "Luxury Cairo Apartment",
                City = "Cairo",
                PricePerDay = 200,
                NumberOfBedRooms = 3,
                IsAvailable = true
            }, owner.Id),

            Apartment.Create(new CreateApartmentDto
            {
                Title = "Budget Cairo Studio",
                City = "Cairo",
                PricePerDay = 50,
                NumberOfBedRooms = 1,
                IsAvailable = true
            }, owner.Id),

            Apartment.Create(new CreateApartmentDto
            {
                Title = "Alexandria Sea View",
                City = "Alexandria",
                PricePerDay = 150,
                NumberOfBedRooms = 2,
                IsAvailable = true
            }, owner.Id)
        };

            context.Apartments.AddRange(apartments);
            context.SaveChanges();
        }

        private ISupportedLanguagePolicy SupportedLanguagePolicy()
        {
            var mock = new Mock<ISupportedLanguagePolicy>();
            mock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            return mock.Object;
        }

        // إعادة تعيين قاعدة البيانات بين الاختبارات
        public void ResetDatabase()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LaylaContext>();

            db.Database.EnsureDeleted();
            db.Database.Migrate();
            SeedTestData(db);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}

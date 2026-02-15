using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.LanguageServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;


namespace LaylaApi.Test.Services.ApartmentServiceTests.IntegrationTests
{
    public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                builder.ConfigureServices(services =>
                {
                    // إزالة تسجيل LaylaContext القديم
                    var dbContextDescriptor = services
                        .SingleOrDefault(d =>
                            d.ServiceType ==
                            typeof(DbContextOptions<LaylaContext>));

                    if (dbContextDescriptor != null)
                        services.Remove(dbContextDescriptor);


                    // إعادة تسجيل LaylaContext بقاعدة اختبارية
                    services.AddDbContext<LaylaContext>(options =>
                            options.UseSqlServer("Server=localhost;Database=XUnitTestLaylaDb;Trusted_Connection=True;TrustServerCertificate=True;",
                            sqlOptions => sqlOptions.UseNetTopologySuite()));
                    // إنشاء provider مؤقت وتهيئة قاعدة البيانات
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<LaylaContext>();

                    context.Database.EnsureDeleted(); // تنظيف كامل قبل كل مرة
                    context.Database.EnsureCreated(); // إنشاء جديد

                    // إضافة بيانات اختبارية
                    SeedTestData(context);

                    context.SaveChanges();
                });
            });
        }

        private static void SeedTestData(LaylaContext context)
        {
            var policy = CreateLanguagePolicyMock();

            var owner = User.Create(
                "Test Owner",
                "owner@test.com",
                "+1000000000",
                "Password123!",
                "hash",
                "en",
                "token",
                policy);

            context.Users.Add(owner);
            context.SaveChanges();

            var apartments = new[]
            {
            Apartment.Create(CreateValidDto(), owner.Id),
            Apartment.Create(CreateValidDto(), owner.Id),
            Apartment.Create(CreateValidDto(), owner.Id)
        };

            context.Apartments.AddRange(apartments);
            context.SaveChanges();
        }

        private static CreateApartmentDto CreateValidDto()
        {
            return new CreateApartmentDto
            {
                Title = "Test Apartment",
                Description = "Integration Test",
                Area = 120,
                Finishing = Apartment.Amenities.Luxury,
                FloorNumber = 2,
                HasElevator = true,
                HasParking = true,
                HasPool = false,
                NumberOfBalconies = 1,
                NumberOfBathrooms = 2,
                NumberOfBedRooms = 3,
                NumberOfLivingRooms = 1,
                NumberOfReceptionRooms = 1,
                Orientation = "North",
                View = Apartment.ApartmentView.SeaView,
                Type = Apartment.BuildingType.Apartment,
                PricePerDay = 100,
                PricePerHour = 10,
                IsAvailable = true,
                Street = "Main Street",
                BuildingNumber = "10",
                ApartmentNumber = "5A",
                City = "Cairo",
                District = "Nasr City",
                Country = "Egypt",
                Latitude = 30.0444,
                Longitude = 31.2357
            };
        }

        private static ISupportedLanguagePolicy CreateLanguagePolicyMock()
        {
            var mock = new Mock<ISupportedLanguagePolicy>();
            mock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            return mock.Object;
        }
    }
}

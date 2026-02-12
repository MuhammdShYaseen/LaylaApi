using FluentAssertions;
using LaylaApi.DataAccess;
using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DynamicApartmentSearchService.BuilderServices;
using LaylaApi.Services.DynamicApartmentSearchService;
using Microsoft.EntityFrameworkCore;
using Moq;
using LaylaApi.Services.LanguageServices;
using LaylaApi.Test.Services.MokeDbContext;


namespace LaylaApi.Test.Services.ApartmentServiceTests
{
    public class ApartmentSearchServiceTests
    {

        [Fact]
        public async Task SearchAsync_ShouldCallFilterBuilder()
        {
            var context = CreateDbContext();

            var repo = CreateRepository(context);

            var filterMock = new Mock<IApartmentFilterBuilder>();
            filterMock
                .Setup(f => f.Build(It.IsAny<IQueryable<Apartment>>(),
                                    It.IsAny<ApartmentSearchRequestDto>()))
                .Returns<IQueryable<Apartment>, ApartmentSearchRequestDto>((q, r) => q);

            var service = new ApartmentSearchService(repo, filterMock.Object);

            await service.SearchAsync(new ApartmentSearchRequestDto(), CancellationToken.None);

            filterMock.Verify(f => f.Build(It.IsAny<IQueryable<Apartment>>(),
                                           It.IsAny<ApartmentSearchRequestDto>()),
                              Times.Once);
        }

        [Fact]
        public async Task SearchAsync_ShouldClampPageSize_AndPageNumber()
        {
            var context = CreateDbContext();

            context.Apartments.AddRange(CreateApartments(10));
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var filter = new ApartmentFilterBuilder(new NetTopologySuite.Geometries.GeometryFactory());

            var service = new ApartmentSearchService(repo, filter);

            var request = new ApartmentSearchRequestDto
            {
                PageSize = 500,
                PageNumber = -5
            };

            var result = await service.SearchAsync(request, CancellationToken.None);

            result.PageSize.Should().Be(50);
            result.PageNumber.Should().Be(1);
        }

        

        [Fact]
        public async Task SearchAsync_ShouldReturnCorrectTotalCount()
        {
            var context = CreateDbContext();

            context.Apartments.AddRange(CreateApartments(5));
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var filter = new ApartmentFilterBuilder(new NetTopologySuite.Geometries.GeometryFactory());

            var service = new ApartmentSearchService(repo, filter);

            var result = await service.SearchAsync(new ApartmentSearchRequestDto(), CancellationToken.None);

            result.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task SearchAsync_ShouldProjectCorrectly()
        {
            var context = CreateDbContext();

            var apartment = CreateFullApartment();
            context.Apartments.Add(apartment);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var filter = new ApartmentFilterBuilder(new NetTopologySuite.Geometries.GeometryFactory());

            var service = new ApartmentSearchService(repo, filter);

            var result = await service.SearchAsync(new ApartmentSearchRequestDto(), CancellationToken.None);

            var dto = result.Items.Single();

            dto.Title.Should().Be(apartment.Title);
            dto.PricePerDay.Should().Be(apartment.PricePerDay.Value);
            dto.OwnerName.Should().Be(apartment.Owner.FullName);
            dto.AverageRating.Should().Be(4.5);
        }

        [Fact]
        public async Task SearchAsync_ShouldCalculateAverageRating()
        {
            var context = CreateDbContext();
            var repo = CreateRepository(context);
            var apartment = CreateFullApartment();
            apartment.Reviews.Add(Review.Create(1,1,4,""));
            apartment.Reviews.Add(Review.Create(1,1,2,""));

            context.Apartments.Add(apartment);
            await context.SaveChangesAsync();

            
            var filter = new ApartmentFilterBuilder(new NetTopologySuite.Geometries.GeometryFactory());

            var service = new ApartmentSearchService(repo, filter);

            var result = await service.SearchAsync(new ApartmentSearchRequestDto(), CancellationToken.None);

            result.Items.Single().AverageRating.Should().Be(3.75);
        }

        

        [Fact]
        public async Task SearchAsync_ShouldSortByPriceAscending()
        {
            var context = CreateDbContext();

            context.Apartments.Add(CreateApartmentWithPrice(100));
            context.Apartments.Add(CreateApartmentWithPrice(50));
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var filter = new ApartmentFilterBuilder(new NetTopologySuite.Geometries.GeometryFactory());

            var service = new ApartmentSearchService(repo, filter);

            var request = new ApartmentSearchRequestDto
            {
                SortBy = "PricePerDay",
                SortDirection = ApartmentSearchRequestDto.SortDirections.Asc
            };

            var result = await service.SearchAsync(request, CancellationToken.None);

            result.Items.First().PricePerDay.Should().Be(50);
        }

        private Apartment CreateApartmentWithPrice(int price)
        {
            var dto = CreateValidDto(pricePerDay: price, pricePerHour: price / 10m, title: $"Apartment {price}");
            var apartment = Apartment.Create(dto, ownerId: 1);

            apartment.Owner = CreateUserEntity(1);

            apartment.MediaFiles = new List<MediaFile>();
            apartment.Reviews = new List<Review>();

            return apartment;
        }



        #region Helper
        private Apartment CreateFullApartment()
        {
            var dto = CreateValidDto();

            var apartment = Apartment.Create(dto, ownerId: 1);

            var owner = CreateUserEntity(1);
            apartment.Owner = owner;

            apartment.MediaFiles = new List<MediaFile>
            {
                 MediaFile.Create(apartment.Id, "https://img1.jpg"),
                 MediaFile.Create(apartment.Id, "https://video1.mp4", "video")
            };

            apartment.Reviews = new List<Review>
            {
                Review.Create(userId: 2, apartmentId: apartment.Id, rating: 4, comment: "Good"),
                Review.Create(userId: 3, apartmentId: apartment.Id, rating: 5, comment: "Excellent")
            };

            return apartment;
        }
        private Apartment[] CreateApartments(int count)
        {
            var list = new List<Apartment>();

            for (int i = 1; i <= count; i++)
            {
                var dto = CreateValidDto(
                    pricePerDay: 50 + i,
                    pricePerHour: 5 + i,
                    title: $"Apartment {i}"
                );

                var apartment = Apartment.Create(dto, ownerId: 1);

                apartment.Owner = CreateUserEntity(1, $"owner{i}@test.com");

                apartment.MediaFiles = new List<MediaFile>();
                apartment.Reviews = new List<Review>();

                list.Add(apartment);
            }

            return list.ToArray();
        }
        private CreateApartmentDto CreateValidDto(decimal pricePerDay = 100, decimal pricePerHour = 10, string title = "Test Apartment")
        {
            return new CreateApartmentDto
            {
                Title = title,
                Description = "Test Description",
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
                Type =  Apartment.BuildingType.Apartment,
                PricePerDay = pricePerDay,
                PricePerHour = pricePerHour,
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

        private ISupportedLanguagePolicy SupportedLanguagePolicy()
        {
            var mock = new Mock<ISupportedLanguagePolicy>();
            mock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            return mock.Object;
        }

        private User CreateUserEntity(int id = 1, string email = "owner@test.com")
        {
            var user = User.Create(
                fullName: "Test Owner",
                email: email,
                phoneNumber: "+201000000000",
                password: "StrongPassword123!",
                passwordHash: "HASH",
                lang: "en",
                emailVerificationToken: Guid.NewGuid().ToString(),
                languagePolicy: SupportedLanguagePolicy()
            );

            //user.Id = id; // لأغراض الاختبار فقط

            return user;
        }

        private static IRepository<Apartment> CreateRepository(LaylaContext context)
        {
            return new Repository<Apartment>(context);
        }

        private static TestLaylaContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<LaylaContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TestLaylaContext(options);
        }
        #endregion
    }
}
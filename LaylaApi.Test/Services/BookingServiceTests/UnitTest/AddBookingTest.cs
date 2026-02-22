using AutoMapper;
using FluentAssertions;
using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Implementations;
using LaylaApi.Services.DynamicApartmentSearchService;
using LaylaApi.Services.LanguageServices;
using LaylaApi.Services.LocationFromIPService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Test.Services.BookingServiceTests.UnitTest
{
    public class AddBookingTest
    {
        [Fact]
        public  async Task AddAsync_ShouldCreateBooking_WhenValid()
        {
            var context = CreateDbContext();
            await AddApartmentAsync_With_Valid_Data(context);
            var dto = ValidCreateBookingDto(1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(5));
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                  .Returns(new BookingDto());

            var service =new BookingService(context,mapper.Object);

            var result = await service.AddAsync(dto, 2, CancellationToken.None);

            result.Should().NotBeNull();
            context.Bookings.Count().Should().Be(1);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenApartmentDoesNotExist()
        {
            var context = CreateDbContext();
            //await AddApartmentAsync_With_Valid_Data(context);
            var dto = ValidCreateBookingDto(1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(5));
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                  .Returns(new BookingDto());
            var service = new BookingService(context, mapper.Object);
            await Assert.ThrowsAsync<BadHttpRequestException>(
                () => service.AddAsync(dto, 2, CancellationToken.None));
        }
        [Fact]
        public async Task AddAsync_ShouldThrow_WhenOwnerBooksOwnApartment()
        {
            var context = CreateDbContext();
            await AddApartmentAsync_With_Valid_Data(context);
            var dto = ValidCreateBookingDto(1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(5));
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                  .Returns(new BookingDto());

            var service = new BookingService(context, mapper.Object);

            await Assert.ThrowsAsync<BadHttpRequestException>(
                () => service.AddAsync(dto, 1, CancellationToken.None));
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenRenterDoesNotExist()
        {
            var context = CreateDbContext();
            await AddApartmentAsync_With_Valid_Data(context);
            var dto = ValidCreateBookingDto(1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(5));
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                  .Returns(new BookingDto());

            var service = new BookingService(context, mapper.Object);

            await Assert.ThrowsAsync<BadHttpRequestException>(
                () => service.AddAsync(dto, 999, CancellationToken.None));
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenDatesOverlap()
        {
            var context = CreateDbContext();
            await AddApartmentAsync_With_Valid_Data(context);

            var dto = ValidCreateBookingDto(1, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(5));
            var dto2 = ValidCreateBookingDto(1, DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(6));
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                  .Returns(new BookingDto());
            var service = new BookingService(context, mapper.Object);

            var addBooking1 = await service.AddAsync(dto, 2,CancellationToken.None);
            addBooking1 = await service.UpdateStatusAsync(1, BookingStatus.Accepted, 1, false, CancellationToken.None);
            addBooking1 = await service.UpdateStatusAsync(1, BookingStatus.Confirmed, 1, false, CancellationToken.None);   

            await Assert.ThrowsAsync<BadHttpRequestException>(
                () => service.AddAsync(dto2,3, CancellationToken.None));
        }

        [Fact]
        public async Task IsDateAvailable_ShouldThrow_WhenStartAfterEnd()
        {
            var context = CreateDbContext();
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                   .Returns(new BookingDto());
            var service = new BookingService(context, mapper.Object);

            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                service.IsDateAvailableAsync(1,
                    DateTime.UtcNow.AddDays(5),
                    DateTime.UtcNow.AddDays(2), CancellationToken.None));
        }

        [Fact]
        public async Task IsDateAvailable_ShouldThrow_WhenStartInPast()
        {
            var context = CreateDbContext();
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                   .Returns(new BookingDto());
            var service = new BookingService(context, mapper.Object);

            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                service.IsDateAvailableAsync(1,
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow.AddDays(2), CancellationToken.None));
        }

        [Fact]
        public async Task IsDateAvailable_ShouldReturnTrue_WhenNoOverlap()
        {
            var context = CreateDbContext();
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                   .Returns(new BookingDto());
            var service = new BookingService(context, mapper.Object);

            var result = await service.IsDateAvailableAsync(
                1,
                DateTime.UtcNow.AddDays(10),
                DateTime.UtcNow.AddDays(12), CancellationToken.None);

            result.Should().BeTrue();
        }
        #region Helper
        private static LaylaContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<LaylaContext>()
                     .UseInMemoryDatabase(Guid.NewGuid().ToString())
                     .Options;

            var dispatcherMock = new Mock<IEventDispatcher>();

            return new LaylaContext(options, dispatcherMock.Object);
        }

        private static ISupportedLanguagePolicy SupportedLanguagePolicy()
        {
            var mock = new Mock<ISupportedLanguagePolicy>();

            mock.Setup(x => x.IsSupported("en")).Returns(true);
            mock.Setup(x => x.IsSupported("ar")).Returns(true);
            mock.Setup(x => x.IsSupported(It.IsNotIn("en", "ar"))).Returns(false);

            return mock.Object;
        }
        private static CreateBookingDto ValidCreateBookingDto(int apartmentId, DateTime startDate, DateTime endDate)
        {
            return new CreateBookingDto
            {
                ApartmentId = apartmentId,
                StartDate = startDate,
                EndDate = endDate
            };
        }
        private static CreateApartmentDto ValidCreateApartmentDto()
        {
            return new CreateApartmentDto
            {
                Title = "Test Apartment",
                Country = "Damascus",
                City = "latakia",
                BuildingNumber = "rt6878",
                Street = "uyy77",
                District = "k890",
                Latitude = 33.5,
                Longitude = 36.3,
                PricePerDay = 50,
                PricePerHour = 5,
                ApartmentNumber = "iop",
                Description = "Test Apartment Des",
                Finishing = Apartment.Amenities.Basic,
                FloorNumber = 2,
                HasElevator = true,
                HasParking = true,
                HasPool = true,
                Area = 23,
                IsAvailable = true,
                IsChatEnabled = true,
                NumberOfBalconies = 1,
                NumberOfBathrooms = 1,
                NumberOfBedRooms = 1,
                NumberOfLivingRooms = 1,
                NumberOfReceptionRooms = 1,
                Orientation = "West Est",
                Type = Apartment.BuildingType.Apartment,
                View = Apartment.ApartmentView.SeaView,
            };
        }

        private async Task AddApartmentAsync_With_Valid_Data(LaylaContext context)
        {
            var registerRequest = new RegisterRequest
            {
                Email = "m@b.com",
                FullName = "Test",
                Lang = "en",
                Password = "Password",
                PhoneNumber = "+963988905898",
            };
            context.Users.Add(User.Create(registerRequest.FullName, registerRequest.Email, registerRequest.PhoneNumber, registerRequest.Password, registerRequest.Password, registerRequest.Lang, "", SupportedLanguagePolicy()));
            await context.SaveChangesAsync();

            var registerRequestRenter = new RegisterRequest
            {
                Email = "renter@b.com",
                FullName = "Test",
                Lang = "en",
                Password = "Password",
                PhoneNumber = "+963988000898",
            };
            context.Users.Add(User.Create(registerRequest.FullName, registerRequest.Email, registerRequest.PhoneNumber, registerRequest.Password, registerRequest.Password, registerRequest.Lang, "", SupportedLanguagePolicy()));
            await context.SaveChangesAsync();

            var registerRequestRenter2 = new RegisterRequest
            {
                Email = "renter@b.com",
                FullName = "Test",
                Lang = "en",
                Password = "Password",
                PhoneNumber = "+963988111898",
            };
            context.Users.Add(User.Create(registerRequest.FullName, registerRequest.Email, registerRequest.PhoneNumber, registerRequest.Password, registerRequest.Password, registerRequest.Lang, "", SupportedLanguagePolicy()));
            await context.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<ApartmentDto>(It.IsAny<Apartment>()))
                  .Returns(new ApartmentDto());

            var service = new ApartmentService(context, mapper.Object, MockILocationFromIP(), MockApartmentSearchService());

            var dto = ValidCreateApartmentDto();

            // Act
            var result = await service.AddAsync(dto, 1, CancellationToken.None);
        }

        private static ILocationFromIPExternalService MockILocationFromIP()
        {
            var mock = new Mock<ILocationFromIPExternalService>();
            return mock.Object;
        }

        private static IApartmentSearchService MockApartmentSearchService()
        {
            var mock = new Mock<IApartmentSearchService>();
            return mock.Object;
        }

        #endregion
    }
}

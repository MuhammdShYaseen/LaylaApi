using AutoMapper;
using FluentAssertions;
using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Implementations;
using LaylaApi.Services.LanguageServices;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Diagnostics;


namespace LaylaApi.Test.Services.ApartmentServiceTests
{
    public class AddAsyncTest
    {
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
                ApartmentNumber ="iop"
            };
        }

        [Fact]
        public async Task AddAsync_WhenDtoIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var context = CreateDbContext();
            var mapper = new Mock<IMapper>();
            var service = new ApartmentService(context, mapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.AddAsync(null!, 1)
            );
        }
        [Fact]
        public async Task AddAsync_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var context = CreateDbContext();
            var mapper = new Mock<IMapper>();
            var service = new ApartmentService(context, mapper.Object);

            var dto = ValidCreateApartmentDto();

            // Act & Assert
            await Assert.ThrowsAsync < KeyNotFoundException>(() =>
                service.AddAsync(dto, userId: 999)
            );
        }

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateApartmentAndReturnDto()
        {
            // Arrange
            var context = CreateDbContext();
            var registerRequest = new RegisterRequest
            {
                Email = "m@b.com",
                FullName = "Test",
                Lang = "en",
                Password = "Password",
                PhoneNumber = "+963988905898",
            };
            context.Users.Add(User.Create(registerRequest, "", "", SupportedLanguagePolicy()));
            await context.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<ApartmentDto>(It.IsAny<Apartment>()))
                  .Returns(new ApartmentDto());

            var service = new ApartmentService(context, mapper.Object);

            var dto = ValidCreateApartmentDto();

            // Act
            var result = await service.AddAsync(dto, 1);

            // Assert
            context.Apartments.Should().HaveCount(1);
            result.Should().NotBeNull();
        }
    }
}

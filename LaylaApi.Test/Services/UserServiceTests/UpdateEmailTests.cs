using AutoMapper;
using FluentAssertions;
using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Implementations;
using LaylaApi.Services.LanguageServices;
using LaylaApi.ValueObjects.UserValueObject;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Services.UserServiceTests
{
    public class UpdateEmailTests
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

        private static User CreateUser(string email)
        {
            return User.Create("Test", email, "+963988905898", "Password", "Password", "en", "", SupportedLanguagePolicy());
        }

        [Fact]
        public async Task UpdateEmailAsync_WhenUserNotFound_ShouldReturnNull()
        {
            var context = CreateDbContext();
            var mapper = new Mock<IMapper>();
            var service = new UserService(context, SupportedLanguagePolicy(), mapper.Object);

            var result = await service.UpdateEmailAsync(1,1, true, "test@test.com", CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateEmailAsync_WhenNotAdminAndEditingOtherUser_ShouldThrowUnauthorized()
        {
            var context = CreateDbContext();
            context.Users.Add(CreateUser("a@test.com"));
            await context.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            var service = new UserService(context, SupportedLanguagePolicy(), mapper.Object);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.UpdateEmailAsync( targetUserId: 1, currentUserId: 2, isAdmin: false, newEmail: "b@test.com", CancellationToken.None)
            );
        }
        [Fact]
        public async Task UpdateEmailAsync_WhenEmailIsSame_ShouldReturnDtoWithoutSaving()
        {
            var context = CreateDbContext();
            context.Users.Add(CreateUser("test@test.com"));
            await context.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<UpdateUserDto>(It.IsAny<User>()))
                  .Returns(new UpdateUserDto());

            var service = new UserService(context,SupportedLanguagePolicy(), mapper.Object);

            var result = await service.UpdateEmailAsync(1,2, true, "  TEST@test.com ", CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateEmailAsync_WhenEmailAlreadyUsed_ShouldThrowArgumentException()
        {
            var context = CreateDbContext();
            context.Users.Add(CreateUser("a@test.com"));
            context.Users.Add(CreateUser("b@test.com"));
            await context.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            var service = new UserService(context,SupportedLanguagePolicy(), mapper.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateEmailAsync(1, 1, true, "b@test.com", CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdateEmailAsync_WithValidEmail_ShouldSetPendingEmail_AndNotChangeEmail()
        {
            var context = CreateDbContext();
            context.Users.Add(CreateUser("old@test.com"));
            await context.SaveChangesAsync();

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<UpdateUserDto>(It.IsAny<User>()))
                  .Returns(new UpdateUserDto());

            var service = new UserService(context, SupportedLanguagePolicy(), mapper.Object);

            await service.UpdateEmailAsync(
                targetUserId: 1,
                currentUserId: 1,
                isAdmin: true,
                newEmail: "new@test.com",CancellationToken.None
            );

            var user = context.Users.Single();

            user.Email!.Value.Should().Be("old@test.com");
            user.PendingEmail.Should().Be("new@test.com");
        }
    }
}

using LaylaApi.DomainEvents.Domain.Exceptions;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Domain
{
    public class ApartmentTests
    {
        private static CreateApartmentDto ValidDto()
        {
            return new CreateApartmentDto
            {
                Title = "Test Apartment",
                Description = "Nice place",
                Country = "Syria",
                IsChatEnabled = true,
                ApartmentNumber = "56fff",
                City = "Damascus",
                BuildingNumber = "g6555",
                Latitude = 33.5,
                Longitude = 36.3,
                PricePerDay = 50,
                PricePerHour = 5,
                District = "rt eet ssse",
                Street = "rt sssw",
                IsAvailable = true
            };
        }

        [Fact]
        public void Create_WhenPricePerDayIsZero_ShouldThrowDomainException()
        {
            var dto = ValidDto();
            dto.PricePerDay = 0;

            Assert.Throws<BadHttpRequestException>(() =>
                Apartment.Create(dto, 1)
            );
        }

        [Fact]
        public void Create_WhenPricePerHourIsNegative_ShouldThrowDomainException()
        {
            var dto = ValidDto();
            dto.PricePerHour = -1;

            Assert.Throws<BadHttpRequestException>(() =>
                Apartment.Create(dto, 1)
            );
        }

        [Fact]
        public void Create_WhenLatitudeIsOutOfRange_ShouldThrowDomainException()
        {
            var dto = ValidDto();
            dto.Latitude = 120;

            Assert.Throws<BadHttpRequestException>(() =>
                Apartment.Create(dto, 1)
            );
        }

        [Fact]
        public void Create_WhenLongitudeIsOutOfRange_ShouldThrowDomainException()
        {
            var dto = ValidDto();
            dto.Longitude = -200;

            Assert.Throws<BadHttpRequestException>(() =>
                Apartment.Create(dto, 1)
            );
        }

        [Fact]
        public void Create_WhenTitleIsEmpty_ShouldThrowDomainException()
        {
            var dto = ValidDto();
            dto.Title = string.Empty;

            Assert.Throws<BadHttpRequestException>(() =>
                Apartment.Create(dto, 1)
            );
        }
    }
}

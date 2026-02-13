using FluentAssertions;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Services.ApartmentServiceTests
{
    public class ApartmentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ApartmentsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Search_WithCityFilter_ReturnsExpectedApartments()
        {
            // Arrange
            var request = new ApartmentSearchRequestDto
            {
                City = "Cairo",
                PageSize = 20
            };

            // Act
            var response = await _client.GetAsync($"/api/apartments/dynamic?City={request.City}&PageSize={request.PageSize}");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ApartmentDto>>>();
            content.Success.Should().BeTrue();
            content.Data.Items.Should().AllSatisfy(a => a.City.Should().Be("Cairo"));
        }

        [Fact]
        public async Task Search_WithPriceRange_ReturnsApartmentsWithinRange()
        {
            // Arrange
            var minPrice = 100;
            var maxPrice = 200;

            // Act
            var response = await _client.GetAsync($"/api/apartments/dynamic?MinPrice={minPrice}&MaxPrice={maxPrice}");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ApartmentDto>>>();
            content.Data.Items.Should().AllSatisfy(a =>
            {
                a.PricePerDay.Should().BeGreaterOrEqualTo(minPrice);
                a.PricePerDay.Should().BeLessOrEqualTo(maxPrice);
            });
        }
    }
}

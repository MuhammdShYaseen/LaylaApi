using FluentAssertions;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.GenericResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LaylaApi.Test.Services.ApartmentServiceTests.IntegrationTests
{
    public class ApartmentsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApartmentsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
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
            content!.Success.Should().BeTrue();
            content!.Data!.Items.Should().AllSatisfy(a => a.City.Should().Be("Cairo"));
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
            content!.Data!.Items.Should().AllSatisfy(a =>
            {
                a.PricePerDay.Should().BeGreaterThanOrEqualTo(minPrice);
                a.PricePerDay.Should().BeLessThanOrEqualTo(maxPrice);
            });
        }
        [Fact]
        public async Task Search_WithCityAndPrice_ReturnsCorrectResults()
        {
            var response = await _client.GetAsync(
                "/api/apartments/dynamic?City=Cairo&MinPrice=100&MaxPrice=200");

            response.EnsureSuccessStatusCode();

            var content = await response.Content
                .ReadFromJsonAsync<ApiResponse<PagedResult<ApartmentDto>>>();

            content!.Data!.Items.Should().AllSatisfy(a =>
            {
                a.City.Should().Be("Cairo");
                a.PricePerDay.Should().BeInRange(100, 200);
            });
        }

        [Fact]
        public async Task Search_WithPagination_DoesNotRepeatResults()
        {
            var pageSize = 5;

            var page1 = await _client.GetFromJsonAsync<
                ApiResponse<PagedResult<ApartmentDto>>>(
                $"/api/apartments/dynamic?Page=1&PageSize={pageSize}");

            var page2 = await _client.GetFromJsonAsync<
                ApiResponse<PagedResult<ApartmentDto>>>(
                $"/api/apartments/dynamic?Page=2&PageSize={pageSize}");

            page1!.Success!.Should().BeTrue();
            page2!.Success!.Should().BeTrue();

            var ids1 = page1!.Data!.Items.Select(x => x.Id);
            var ids2 = page2!.Data!.Items.Select(x => x.Id);

            ids1.Intersect(ids2).Should().BeEmpty();
        }

        [Fact]
        public async Task Search_WithoutFilters_ReturnsResults()
        {
            var response = await _client.GetAsync("/api/apartments/dynamic");

            response.EnsureSuccessStatusCode();

            var content = await response.Content
                .ReadFromJsonAsync<ApiResponse<PagedResult<ApartmentDto>>>();

            content!.Success!.Should().BeTrue();
            content!.Data!.Items.Should().BeEmpty();
        }
        [Fact]
        public async Task Search_WithInvalidPrice_DoesNotCrash()
        {
            var response = await _client.GetAsync(
                "/api/apartments/dynamic?MinPrice=-100&MaxPrice=-50");

            response.EnsureSuccessStatusCode();

            var content = await response.Content
                .ReadFromJsonAsync<ApiResponse<PagedResult<ApartmentDto>>>();

            content!.Success!.Should().BeTrue();
        }

        [Fact]
        public async Task Search_WithExactPriceBoundary_IncludesResult()
        {
            var response = await _client.GetAsync(
                "/api/apartments/dynamic?MinPrice=100&MaxPrice=100");

            response.EnsureSuccessStatusCode();

            var content = await response.Content
                .ReadFromJsonAsync<ApiResponse<PagedResult<ApartmentDto>>>();

            content!.Data!.Items.Should()
                .OnlyContain(a => a.PricePerDay == 100);
        }

    }
}

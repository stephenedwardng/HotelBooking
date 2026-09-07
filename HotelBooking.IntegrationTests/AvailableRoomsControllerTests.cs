using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HotelBooking.DTOs;
using HotelBooking.IntegrationTests;

namespace HotelBooking.IntegrationTests
{
    public class AvailableRoomsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AvailableRoomsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReturnsAvailableRooms()
        {
            var response = await _client.GetAsync(
                "/api/HotelBooking/available-rooms?hotelId=1&start=2026-09-20&end=2026-09-21&guests=2");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var rooms = await response.Content.ReadFromJsonAsync<List<AvailableRoom>>();

            rooms.Should().NotBeNull();
            rooms.Should().NotBeEmpty();
        }
    }
}

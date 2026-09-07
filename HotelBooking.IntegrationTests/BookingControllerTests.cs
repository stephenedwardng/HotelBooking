using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HotelBooking.DTOs;
using HotelBooking.IntegrationTests;
using HotelBooking.Models;

namespace HotelBooking.IntegrationTests
{
    public class BookingControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BookingControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CanBookRoomSuccessfully()
        {
            Console.WriteLine(typeof(Program).Assembly.Location);

            var request = new BookRoomRequest
            {
                HotelId = 1,
                RoomId = 1,
                RoomTypeId = 2,
                GuestId = 1,
                NoOfGuests = 2,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3)
            };

            var response = await _client.PostAsJsonAsync("/api/HotelBooking/book", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var booking = await response.Content.ReadFromJsonAsync<BookingDto>();

            booking.Should().NotBeNull();
            booking.Reference.Should().NotBeNullOrWhiteSpace();
        }
    }
}

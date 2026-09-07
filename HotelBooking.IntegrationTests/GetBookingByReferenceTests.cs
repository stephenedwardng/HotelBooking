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
    public class GetBookingByReferenceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public GetBookingByReferenceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RetrievesBookingByReference()
        {
            // Create booking
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

            var createResponse = await _client.PostAsJsonAsync("/api/HotelBooking/book", request);
            var booking = await createResponse.Content.ReadFromJsonAsync<BookingDto>();

            // Retrieve booking
            var getResponse = await _client.GetAsync($"/api/HotelBooking/{booking.Reference}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var details = await getResponse.Content.ReadFromJsonAsync<BookingDetails>();

            details.Should().NotBeNull();
            details.Reference.Should().Be(booking.Reference);
        }
    }
}

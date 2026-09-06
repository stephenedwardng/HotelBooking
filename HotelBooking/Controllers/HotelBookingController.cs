using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HotelBooking.Controllers
{
    /// <summary>
    /// Controller for handling hotel booking operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HotelBookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly RoomService _roomService;
        private readonly HotelService _hotelService;

        public HotelBookingController(
            BookingService bookingService,
            RoomService roomService,
            HotelService hotelService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _hotelService = hotelService;
        }

        /// <summary>
        /// Finds a hotel by its name.
        /// </summary>
        /// <param name="name">The name of the hotel to find.</param>
        /// <returns>The hotel if found, otherwise not found.</returns>
        [HttpGet("hotel")]
        public async Task<ActionResult<Hotel>> FindHotelByName(string name)
        {
            var hotel = await _hotelService.FindHotelByNameAsync(name);
            return hotel == null ? NotFound("Hotel not found.") : hotel;
        }

        /// <summary>
        /// Finds available rooms in a hotel for a given date range and number of guests.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel.</param>
        /// <param name="start">The start date of the booking period.</param>
        /// <param name="end">The end date of the booking period.</param>
        /// <param name="guests">The number of guests.</param>
        /// <returns>The list of available rooms.</returns>
        /// <returns></returns>
        [HttpGet("available-rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> FindAvailableRooms(
            int hotelId, DateTime start, DateTime end, int guests)
        {
            var rooms = await _roomService.FindAvailableRoomsAsync(hotelId, start, end, guests);
            return Ok(rooms);
        }

        /// <summary>
        /// Books a room based on the provided request details.
        /// </summary>
        /// <param name="request">The booking request details.</param>
        /// <returns>The created booking reference.</returns>
        [HttpPost("book")]
        public async Task<ActionResult<Booking>> BookRoom([FromQuery] BookRoomRequest request)
        {
            var booking = await _bookingService.BookRoomAsync(request);

            return CreatedAtAction(nameof(GetBookingByReference),
                new { reference = booking.Reference },
                booking);
        }

        /// <summary>
        /// Retrieves a booking by its reference.
        /// </summary>
        /// <param name="reference">The reference of the booking to retrieve.</param>
        /// <returns>The booking if found, otherwise not found.</returns>
        [HttpGet("booking")]
        public async Task<ActionResult<Booking>> GetBookingByReference(string reference)
        {
            var booking = await _bookingService.GetBookingByReferenceAsync(reference);
            return booking == null ? NotFound("Booking not found.") : booking;
        }
    }

}

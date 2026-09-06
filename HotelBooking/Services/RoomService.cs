using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    /// <summary>
    /// Service for room operations
    /// </summary>
    public class RoomService
    {
        private readonly HotelBookingDbContext _db;

        public RoomService(HotelBookingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Finds available rooms in a hotel for a given date range and number of guests
        /// </summary>
        /// <param name="hotelId">ID of the hotel</param>
        /// <param name="start">Start date of the booking period</param>
        /// <param name="end">End date of the booking period</param>
        /// <param name="guests">Number of guests</param>
        /// <returns>List of available rooms</returns>
        public async Task<IEnumerable<Room>> FindAvailableRoomsAsync(int hotelId, DateTime start, DateTime end, int guests)
        {
            var rooms = await _db.Rooms
                .Where(r => r.HotelId == hotelId)
                .Join(_db.RoomTypes,
                      room => room.RoomTypeId,
                      type => type.Id,
                      (room, type) => new { room, type })
                .Where(rt => rt.type.Capacity >= guests)
                .Select(rt => rt.room)
                .ToListAsync();

            return rooms.Where(room =>
                !_db.Bookings.Any(b =>
                    b.RoomId == room.Id &&
                    b.StartDate < end &&
                    b.EndDate > start));
        }
    }

}

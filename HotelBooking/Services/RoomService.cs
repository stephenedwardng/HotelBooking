using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.DTOs;
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
        /// Finds available rooms for a given hotel, date range, no of guests.
        /// </summary>
        /// <param name="hotelId">Id of the hotel</param>
        /// <param name="start">Start date of the booking</param>
        /// <param name="end">End date of the booking</param>
        /// <param name="guests">No of guests</param>
        /// <returns>List of available rooms</returns>
        public async Task<IEnumerable<AvailableRoom>> FindAvailableRoomsAsync(
            int hotelId, DateTime start, DateTime end, int guests)
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

            var availableRooms = rooms.Where(room =>
                !_db.Bookings.Any(b =>
                    b.RoomId == room.Id &&
                    b.StartDate < end &&
                    b.EndDate > start));

            return availableRooms.Select(r => new AvailableRoom
            {
                Id = r.Id,
                HotelId = r.HotelId,
                RoomTypeId = r.RoomTypeId
            });
        }

    }

}

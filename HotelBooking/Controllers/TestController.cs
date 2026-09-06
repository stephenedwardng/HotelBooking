using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Controllers
{
    /// <summary>
    /// Controller for managing test data, resetting and seeding db
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestDataController : ControllerBase
    {
        private readonly HotelBookingDbContext _context;

        public TestDataController(HotelBookingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Resets db by clearing all data from tables and resetting the identity sequence
        /// </summary>
        /// <returns></returns>
        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            _context.ChangeTracker.Clear();

            _context.Bookings.RemoveRange(_context.Bookings);
            _context.Rooms.RemoveRange(_context.Rooms);
            _context.RoomTypes.RemoveRange(_context.RoomTypes);
            _context.Guests.RemoveRange(_context.Guests);
            _context.Hotels.RemoveRange(_context.Hotels);

            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence");

            return Ok("Database reset");
        }

        /// <summary>
        /// Seeds db with test data
        /// </summary>
        /// <returns></returns>
        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            _context.ChangeTracker.Clear();

            // Room Types
            var single = new RoomType { Name = "Single", Capacity = 1 };
            var doubleRoom = new RoomType { Name = "Double", Capacity = 2 };
            var deluxe = new RoomType { Name = "Deluxe", Capacity = 4 };

            _context.RoomTypes.AddRange(single, doubleRoom, deluxe);
            await _context.SaveChangesAsync();

            // Hotels
            var hotel1 = new Hotel { Name = "Novotel", NoOfRooms = 6 };
            var hotel2 = new Hotel { Name = "Premier Inn", NoOfRooms = 6 };

            _context.Hotels.AddRange(hotel1, hotel2);
            await _context.SaveChangesAsync();

            // Rooms (6 per hotel)
            List<Room> rooms = new();

            RoomType[] types = new[] { single, doubleRoom, deluxe };

            void AddRoomsForHotel(Hotel hotel)
            {
                for (int i = 0; i < 6; i++)
                {
                    var type = types[i % 3]; // rotate Single, Double, Deluxe
                    rooms.Add(new Room
                    {
                        HotelId = hotel.Id,
                        RoomTypeId = type.Id
                    });
                }
            }

            AddRoomsForHotel(hotel1);
            AddRoomsForHotel(hotel2);

            _context.Rooms.AddRange(rooms);
            await _context.SaveChangesAsync();

            // Guests
            var guest1 = new Guest { Name = "Stephen" };
            var guest2 = new Guest { Name = "Eddie" };

            _context.Guests.AddRange(guest1, guest2);
            await _context.SaveChangesAsync();

            return Ok("Database seeded with 2 hotels, 3 room types and 12 rooms");
        }
    }

}

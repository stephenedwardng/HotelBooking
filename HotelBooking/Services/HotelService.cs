using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    /// <summary>
    /// Service for hotel
    /// </summary>
    public class HotelService
    {
        private readonly HotelBookingDbContext _db;

        public HotelService(HotelBookingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Finds a hotel by its name
        /// </summary>
        /// <param name="name">Name of hotel to find</param>
        /// <returns>Hotel if found, otherwise null</returns>
        public async Task<Hotel?> FindHotelByNameAsync(string name)
        {
            return await _db.Hotels
                .FirstOrDefaultAsync(h => h.Name.ToLower() == name.ToLower());
        }
    }

}

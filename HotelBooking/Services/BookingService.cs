using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Models;
using HotelBooking.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotelBooking.Services
{
    /// <summary>
    /// Service for handling hotel room bookings
    /// </summary>
    public class BookingService
    {
        private readonly HotelBookingDbContext _db;

        public BookingService(HotelBookingDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Books a room for a guest if all business rules are satisfied
        /// </summary>
        /// <param name="request">Booking request</param>
        /// <returns>Created booking with reference</returns>
        /// <exception cref="BusinessRuleException">Thrown when a business rule is violated</exception>
        public async Task<BookingDto> BookRoomAsync(BookRoomRequest request)
        {
            var room = await _db.Rooms.FindAsync(request.RoomId)
                ?? throw new BusinessRuleException("Room not found");

            var guest = await _db.Guests.FindAsync(request.GuestId)
                ?? throw new BusinessRuleException("Guest not found");

            var roomType = await _db.RoomTypes.FindAsync(request.RoomTypeId)
                ?? throw new BusinessRuleException("Room type not found");

            if (room.HotelId != request.HotelId)
                throw new BusinessRuleException("Room does not belong to the specified hotel");

            if (room.RoomTypeId != request.RoomTypeId)
                throw new BusinessRuleException("Room type mismatch");

            if (request.NoOfGuests > roomType.Capacity)
                throw new BusinessRuleException("Room capacity exceeded");

            if (request.StartDate <= DateTime.Now)
                throw new BusinessRuleException("Start date must not be in the past");

            if (request.EndDate <= request.StartDate)
                throw new BusinessRuleException("End date must be a day or more after start date");

            bool overlap = await _db.Bookings
                .AnyAsync(b =>
                    b.RoomId == request.RoomId &&
                    b.StartDate < request.EndDate &&
                    b.EndDate > request.StartDate);

            if (overlap)
                throw new BusinessRuleException("Room is not available for those dates");

            var booking = new Booking
            {
                RoomId = request.RoomId,
                GuestId = request.GuestId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                NoOfGuests = request.NoOfGuests,
                Reference = Guid.NewGuid().ToString()
            };
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return new BookingDto
            {
                Reference = booking.Reference,
                RoomId = booking.RoomId,
                GuestId = booking.GuestId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                NoOfGuests = booking.NoOfGuests
            };

        }

        /// <summary>
        /// Retrieves booking by reference
        /// </summary> 
        /// <param name="reference">Booking reference</param>
        /// <returns>Booking if found, otherwise null</returns>
        public async Task<BookingDetails?> GetBookingByReferenceAsync(string reference)
        {
            var booking = await _db.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.Reference == reference);

            if (booking == null)
                return null;

            return new BookingDetails
            {
                Reference = booking.Reference,
                RoomId = booking.RoomId,
                GuestId = booking.GuestId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                NoOfGuests = booking.NoOfGuests,
                GuestName = booking.Guest.Name,
                RoomTypeName = booking.Room.RoomType.Name
            };

        }
    }

}

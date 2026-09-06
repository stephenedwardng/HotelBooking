using System;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotelBooking.Tests
{
    public class RoomServiceTests
    {
        private HotelBookingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new HotelBookingDbContext(options);

            // Seed Hotels
            db.Hotels.Add(new Hotel { Id = 1, Name = "Novotel" });
            db.Hotels.Add(new Hotel { Id = 2, Name = "Premier Inn" });

            // Seed Room Types
            db.RoomTypes.Add(new RoomType { Id = 1, Name = "Single", Capacity = 1 });
            db.RoomTypes.Add(new RoomType { Id = 2, Name = "Double", Capacity = 2 });
            db.RoomTypes.Add(new RoomType { Id = 3, Name = "Deluxe", Capacity = 4 });

            // Seed Rooms
            db.Rooms.Add(new Room { Id = 1, HotelId = 1, RoomTypeId = 1 }); // Single
            db.Rooms.Add(new Room { Id = 2, HotelId = 1, RoomTypeId = 2 }); // Double
            db.Rooms.Add(new Room { Id = 3, HotelId = 1, RoomTypeId = 3 }); // Deluxe
            db.Rooms.Add(new Room { Id = 4, HotelId = 2, RoomTypeId = 3 }); // Deluxe (different hotel)

            db.SaveChanges();
            return db;
        }

        private RoomService CreateService(HotelBookingDbContext db)
        {
            return new RoomService(db);
        }

        [Fact]
        public async Task ReturnsRoomsMatchingHotelId()
        {
            var db = CreateDbContext();
            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 1,
                start: DateTime.Today.AddDays(1),
                end: DateTime.Today.AddDays(2),
                guests: 1);

            Assert.All(rooms, r => Assert.Equal(1, r.HotelId));
        }

        [Fact]
        public async Task FiltersOutRoomsWithInsufficientCapacity()
        {
            var db = CreateDbContext();
            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 1,
                start: DateTime.Today.AddDays(1),
                end: DateTime.Today.AddDays(2),
                guests: 4);

            // Only Deluxe has capacity 4
            Assert.Single(rooms);
            Assert.Equal(3, rooms.First().Id);
        }

        [Fact]
        public async Task ExcludesRoomsWithOverlappingBookings()
        {
            var db = CreateDbContext();

            // Add overlapping booking for Deluxe room (Id = 3)
            db.Bookings.Add(new Booking
            {
                RoomId = 3,
                GuestId = 1,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(5),
                NoOfGuests = 2,
                Reference = "TEST"
            });
            db.SaveChanges();

            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 1,
                start: DateTime.Today.AddDays(1),
                end: DateTime.Today.AddDays(3),
                guests: 4);

            // Deluxe is booked → no rooms available for 4 guests
            Assert.Empty(rooms);
        }

        [Fact]
        public async Task ReturnsMultipleAvailableRooms()
        {
            var db = CreateDbContext();
            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 1,
                start: DateTime.Today.AddDays(1),
                end: DateTime.Today.AddDays(2),
                guests: 2);

            // Double + Deluxe both fit 2 guests
            Assert.Equal(2, rooms.Count());
        }

        [Fact]
        public async Task ReturnsEmpty_WhenNoRoomsMatchHotel()
        {
            var db = CreateDbContext();
            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 999,
                start: DateTime.Today.AddDays(1),
                end: DateTime.Today.AddDays(2),
                guests: 2);

            Assert.Empty(rooms);
        }

        [Fact]
        public async Task ReturnsEmpty_WhenNoRoomsMatchCapacity()
        {
            var db = CreateDbContext();
            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 1,
                start: DateTime.Today.AddDays(1),
                end: DateTime.Today.AddDays(2),
                guests: 10);

            Assert.Empty(rooms);
        }

        [Fact]
        public async Task OverlapDetection_WorksForEdgeCase_StartInsideExistingBooking()
        {
            var db = CreateDbContext();

            db.Bookings.Add(new Booking
            {
                RoomId = 3, // Deluxe
                GuestId = 1,
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(6),
                NoOfGuests = 2,
                Reference = "EDGE"
            });
            db.SaveChanges();

            var service = CreateService(db);

            var rooms = await service.FindAvailableRoomsAsync(
                hotelId: 1,
                start: DateTime.Today.AddDays(4), // inside existing booking
                end: DateTime.Today.AddDays(7),
                guests: 4);

            Assert.Empty(rooms);
        }
    }
}

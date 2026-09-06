using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HotelBooking.Tests
{
    public class BookingServiceTests
    {
        private HotelBookingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new HotelBookingDbContext(options);

            db.Hotels.Add(new Hotel { Id = 1, Name = "Novotel" });

            db.RoomTypes.Add(new RoomType { Id = 1, Name = "Double", Capacity = 2 });

            db.Rooms.Add(new Room
            {
                Id = 1,
                HotelId = 1,
                RoomTypeId = 1
            });

            db.Guests.Add(new Guest
            {
                Id = 1,
                Name = "Stephen"
            });

            db.SaveChanges();
            return db;
        }

        private BookRoomRequest CreateValidRequest()
        {
            return new BookRoomRequest
            {
                HotelId = 1,
                RoomId = 1,
                RoomTypeId = 1,
                GuestId = 1,
                NoOfGuests = 2,
                StartDate = DateTime.Now.Date.AddDays(1),
                EndDate = DateTime.Now.Date.AddDays(3)
            };
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenRoomNotFound()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.RoomId = 999;

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenGuestNotFound()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.GuestId = 999;

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenRoomTypeNotFound()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.RoomTypeId = 999;

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenRoomDoesNotBelongToHotel()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.HotelId = 999;

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenRoomTypeMismatch()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.RoomTypeId = 2; // different from room.RoomTypeId

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenCapacityExceeded()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.NoOfGuests = 3; // roomType.Capacity = 2

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenStartDateInPast()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.StartDate = DateTime.Now.AddDays(-1);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenEndDateNotAfterStartDate()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();
            request.EndDate = request.StartDate; // same day

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Throws_WhenOverlapExists()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            // Existing booking that overlaps
            db.Bookings.Add(new Booking
            {
                Id = 1,
                RoomId = 1,
                GuestId = 1,
                StartDate = DateTime.Now.Date.AddDays(2),
                EndDate = DateTime.Now.Date.AddDays(4),
                NoOfGuests = 2,
                Reference = "EXISTING"
            });
            db.SaveChanges();

            var request = CreateValidRequest(); // 1–3, overlaps with 2–4

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                service.BookRoomAsync(request));
        }

        [Fact]
        public async Task BookRoomAsync_Succeeds_WhenAllRulesPass()
        {
            var db = CreateDbContext();
            var service = new BookingService(db);

            var request = CreateValidRequest();

            var result = await service.BookRoomAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.RoomId, result.RoomId);
            Assert.Equal(request.GuestId, result.GuestId);
            Assert.Equal(request.StartDate, result.StartDate);
            Assert.Equal(request.EndDate, result.EndDate);
            Assert.Equal(request.NoOfGuests, result.NoOfGuests);
            Assert.False(string.IsNullOrWhiteSpace(result.Reference));
        }
    }
}
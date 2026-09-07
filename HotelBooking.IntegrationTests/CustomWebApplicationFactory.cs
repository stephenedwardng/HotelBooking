using HotelBooking;
using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove ALL EF Core provider registrations
                var toRemove = services
                    .Where(s =>
                        s.ServiceType == typeof(DbContextOptions<HotelBookingDbContext>) ||
                        s.ServiceType == typeof(DbContextOptions) ||
                        s.ServiceType == typeof(IDbContextFactory<HotelBookingDbContext>) ||
                        (s.ImplementationType != null &&
                         (s.ImplementationType.Name.Contains("Sqlite") ||
                          s.ImplementationType.Name.Contains("SqlServer") ||
                          s.ImplementationType.Name.Contains("Relational"))))
                    .ToList();

                foreach (var descriptor in toRemove)
                    services.Remove(descriptor);

                // Add InMemory provider
                services.AddDbContext<HotelBookingDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestsDb");
                });

                // Build provider so we can seed
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();
                    db.Database.EnsureCreated();

                    // Seed Hotels
                    db.Hotels.Add(new Hotel { Id = 1, Name = "Novotel" });

                    // Seed Room Types
                    db.RoomTypes.Add(new RoomType { Id = 1, Name = "Single", Capacity = 1 });
                    db.RoomTypes.Add(new RoomType { Id = 2, Name = "Double", Capacity = 2 });
                    db.RoomTypes.Add(new RoomType { Id = 3, Name = "Deluxe", Capacity = 4 });

                    // Seed Rooms
                    db.Rooms.Add(new Room { Id = 1, HotelId = 1, RoomTypeId = 2 });
                    db.Rooms.Add(new Room { Id = 2, HotelId = 1, RoomTypeId = 3 });

                    // Seed Guests
                    db.Guests.Add(new Guest { Id = 1, Name = "Test Guest" });

                    db.SaveChanges();
                }
            });
        }
    }
}

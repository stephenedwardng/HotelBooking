using HotelBooking.Data;
using HotelBooking.Middleware;
using HotelBooking.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<HotelBookingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("HotelBookingDb") ?? throw new InvalidOperationException("Connection string 'HotelBookingDb' not found.")));
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<HotelService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.Run();

namespace HotelBooking
{
    public partial class Program { }
}
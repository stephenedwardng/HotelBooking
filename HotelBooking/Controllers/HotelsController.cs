using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HotelBookingDbContext _context;

        public HotelsController(HotelBookingDbContext context)
        {
            _context = context;
        }

        // GET: api/Hotels
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
        //{
        //    return await _context.Hotels.ToListAsync();
        //}

        // GET: api/Hotels/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Hotel>> GetHotel(int id)
        //{
        //    var hotel = await _context.Hotels.FindAsync(id);

        //    if (hotel == null)
        //    {
        //        return NotFound();
        //    }
            
        //    return hotel;
        //}

        // POST: api/Hotels
        //[HttpPost]
        //public async Task<ActionResult<Hotel>> CreateHotel(Hotel hotel)
        //{
        //    _context.Hotels.Add(hotel);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        //}

        // PUT: api/Hotels/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateHotel(int id, Hotel hotel)
        //{
        //    if (id != hotel.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(hotel).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!_context.Hotels.Any(h => h.Id == id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // DELETE: api/Hotels/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteHotel(int id)
        //{
        //    var hotel = await _context.Hotels.FindAsync(id);

        //    if (hotel == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Hotels.Remove(hotel);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}

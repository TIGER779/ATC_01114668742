using System.Security.Claims;
using AreebTechnologyTask.Data;
using AreebTechnologyTask.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AreebTechnologyTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/booking/BookEvent/{eventId}
        [HttpPost("BookEvent/{eventId}")]
        [Authorize]
        public async Task<IActionResult> BookEvent(int eventId)
        {
            // Get user id from Jwt
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID not found in the token.");
            }

            var userId = int.Parse(userIdClaim);

            // Check if event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
            {
                return NotFound(new { message = "Event not found." });
            }

            // handle case double booking
            var alreadyBooked = await _context.Bookings.AnyAsync
                (u => u.UserId == userId && u.EventId == eventId);
            if (alreadyBooked)
            {
                return BadRequest(new { message = "You already booked this event." });
            }


            // Create Booking
            var booking = new Booking
            {
                UserId = userId,
                EventId = eventId,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Booking successful." });

        }

        // GET: api/booking/GetMyBookings
        [HttpGet("GetMyBookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            // Get user id from Jwt
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("User ID not found in the token.");
            }
            var userId = int.Parse(userIdClaim);

            // GetAllBookings for logged user
            var mybookings = await _context.Bookings
                .Include(b => b.Event)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.BookingDate,
                    Event = new
                    {
                        b.Event.Id,
                        b.Event.Name,
                        b.Event.Date,
                        b.Event.Venue,
                        b.Event.Price
                    }

                })
                .ToListAsync();

            return Ok(mybookings);
        }

    }
}

using AreebTechnologyTask.Data;
using AreebTechnologyTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace AreebTechnologyTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        // Admin Side Event Controller (CRUDs)
        /*
         * 1-  GET / (list all events)
         * 2-  POST / (admin only - create)
         * 3-  PUT /{id} (admin only - update)
         * 4-  DELETE /{id} (admin only - delete)
         */


        // POST: api/event/AddEvent -- With Json Body
        [HttpPost("AddEvent")]
        public async Task<ActionResult<Event>> AddEvent(Event NewEvent)
        {
            _context.Events.Add(NewEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventByID), new { id = NewEvent.Id }, NewEvent);
        }
        // PUT: api/event/UpdateEvent/{id} -- With Json Body
        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, Event updatedEvent)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound(new
                {
                    message = "Event Not Found"
                });
            }
            existingEvent.Name = updatedEvent.Name ?? existingEvent.Name;
            existingEvent.Description = updatedEvent.Description ?? existingEvent.Description;
            existingEvent.Category = updatedEvent.Category ?? existingEvent.Category;
            existingEvent.Date = updatedEvent.Date != default ? updatedEvent.Date : existingEvent.Date;
            existingEvent.Venue = updatedEvent.Venue ?? existingEvent.Venue;
            existingEvent.Price = updatedEvent.Price != 0 ? updatedEvent.Price : existingEvent.Price;
            existingEvent.ImageUrl = updatedEvent.ImageUrl ?? existingEvent.ImageUrl;
            existingEvent.Bookings = updatedEvent.Bookings ?? existingEvent.Bookings;

            await _context.SaveChangesAsync();


            return Ok(new { message = "Event Updated successfully" });
        }

        // Delete: api/event/DeleteEvent/{id} -- With Json Body
        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var EventItem = await _context.Events.FindAsync(id);
            if (EventItem == null)
            {
                return NotFound();
            }
            _context.Events.Remove(EventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = " Event Deleted successfully" });
        }

        // User Side Event Controller
        /*
         * 1-  GET / (list all events)
         * 2-  GET /{id} (get event by id)
         */

        // GET: api/event/GetAllEvents
        [HttpGet("GetAllEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        // GET: api/event/GetEventByID/{id}
        [HttpGet("GetEventByID/{id}")]
        public async Task<ActionResult<Event>> GetEventByID(int id)
        {
            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return Ok(eventItem);
        }
    }
}

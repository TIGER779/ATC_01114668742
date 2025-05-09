using System.Collections.Generic;
using System.Security.Claims;
using AreebTechnologyTask.Data;
using AreebTechnologyTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace AreebTechnologyTask.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

       // GET: /Event/AddEvent
        public IActionResult AddEvent()
        {
            return View(); //Views/Event/AddEvent.cshtml
        }

        // POST: /Event/AddEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEvent(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newEvent);
        }

        // GET: /Event/EditEvent/5
        public async Task<IActionResult> EditEvent(int id)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
                return NotFound();

            return View(existingEvent); //Views/Event/EditEvent.cshtml
        }

        // POST: /Event/EditEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, Event updatedEvent)
        {
            if (id != updatedEvent.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(updatedEvent);

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
                return NotFound();

            existingEvent.Name = updatedEvent.Name;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.Category = updatedEvent.Category;
            existingEvent.Date = updatedEvent.Date;
            existingEvent.Venue = updatedEvent.Venue;
            existingEvent.Price = updatedEvent.Price;
            existingEvent.ImageUrl = updatedEvent.ImageUrl;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Event/DeleteEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
                return NotFound();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}


// Admin Side Event Controller (CRUDs)
/*
 * 1-  GET / (list all events)
 * 2-  POST / (admin only - create)
 * 3-  PUT /{id} (admin only - update)
 * 4-  DELETE /{id} (admin only - delete)
 */

// User Side Event Controller
/*   1 - GET / (list all events)
 *   2-  GET /{id} (get event by id)*/



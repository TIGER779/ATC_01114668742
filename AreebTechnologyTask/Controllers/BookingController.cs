using System.Security.Claims;
using AreebTechnologyTask.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class BookingController : Controller
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return RedirectToAction("Login", "Auth");
        }

        var userId = int.Parse(userIdClaim);

        var bookings = await _context.Bookings
            .Include(b => b.Event)
            .Where(b => b.UserId == userId)
            .ToListAsync();

        return View(bookings); //Views/Booking/MyBookings.cshtml
    }

    [HttpGet]
    public async Task<IActionResult> Congratulations(string eventName)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.Name == eventName);
        if (!eventExists)
        {
            return NotFound("Event not found.");
        }

        ViewBag.EventName = eventName;
        return View();
    }
}


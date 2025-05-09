using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AreebTechnologyTask.Data;
using AreebTechnologyTask.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AreebTechnologyTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // Display all events on the home page
        public async Task<IActionResult> Index(string returnUrl)
        {
            var events = await _context.Events.ToListAsync();

            // Check if the user is authenticated via the JWT token
            var token = Request.Cookies["JwtToken"];
            List<int> bookedEventIds = new List<int>();

            if (!string.IsNullOrEmpty(token))
            {
                var userId = GetUserIdFromToken(token); // extract user ID from token
                bookedEventIds = await _context.Bookings
                    .Where(b => b.UserId == userId)
                    .Select(b => b.EventId)
                    .ToListAsync();
            }

            ViewBag.BookedEventIds = bookedEventIds; // Pass the list of booked event IDs
            ViewBag.JwtToken = token; // Pass the JWT token

            ViewBag.ReturnUrl = returnUrl ?? Url.Content("~/");

            return View(events);
        }


        // extract user ID from JWT token
        private int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken != null)
            {
                var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
            }

            return 0;
        }


        // event booking
        [HttpPost]
        public async Task<IActionResult> BookEvent(int eventId)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                if (isAjax)
                    return Json(new { success = false, message = "Unauthorized" });

                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Home") });
            }

            var userId = GetUserIdFromToken(token);
            if (userId == 0)
            {
                if (isAjax)
                    return Json(new { success = false, message = "Invalid token" });

                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Index", "Home") });
            }

            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.EventId == eventId && b.UserId == userId);

            if (existingBooking != null)
            {
                if (isAjax)
                    return Json(new { success = false, message = "Event already booked." });

                TempData["Error"] = "Event already booked.";
                return RedirectToAction("Index", "Home");
            }

            var booking = new Booking
            {
                UserId = userId,
                EventId = eventId,
                BookingDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookedEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

            if (isAjax)
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Congratulations", "Booking", new { eventName = bookedEvent?.Name })
                });
            }

            TempData["Success"] = "Event booked successfully!";
            return RedirectToAction("Congratulations", "Booking", new { eventName = bookedEvent?.Name });
        }


        // GET: /Event/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem); //Views/Event/Details.cshtml
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

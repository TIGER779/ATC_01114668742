using Microsoft.AspNetCore.Mvc;

namespace AreebTechnologyTask.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

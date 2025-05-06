using Microsoft.AspNetCore.Mvc;

namespace AreebTechnologyTask.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    public class AuthController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}

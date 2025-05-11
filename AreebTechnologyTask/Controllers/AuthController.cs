using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AreebTechnologyTask.Data;
using AreebTechnologyTask.Dto;
using AreebTechnologyTask.Enums;
using AreebTechnologyTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace AreebTechnologyTask.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(1); // JWT token lifetime


        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Auth/Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already logged in
            var token = Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                    var userRole = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Redirect based on role
                    if (userRole == UserRole.Admin.ToString())
                    {
                        return RedirectToAction("AdminPanel", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch
                {
                    // If token is invalid, clear it and show login page
                    Response.Cookies.Delete("JwtToken");
                }
            }
            return View();
        }

        // POST: /Auth/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto request, string? returnUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                TempData["LoginError"] = "Email not found. Please check your email or register if you don't have an account.";
                return View(request);
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
            {
                TempData["LoginError"] = "Incorrect password. Please try again.";
                return View(request);
            }

            string token = CreateToken(user, out DateTime expiresAt);

            Response.Cookies.Append("JwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt
            });

            // Redirect based on user role
            if (user.Role == UserRole.Admin)
            {
                return RedirectToAction("AdminPanel", "Admin");
            }
            else
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("JwtToken"); // Remove JWT cookie

            HttpContext.Session.Clear();

            await Task.CompletedTask;

            return RedirectToAction("Index", "Home");
        }

        private string CreateToken(User user, out DateTime expiresAt)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            expiresAt = DateTime.UtcNow.Add(_tokenLifetime);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            // Check if user is already logged in
            var token = Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                    var userRole = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Redirect based on role
                    if (userRole == UserRole.Admin.ToString())
                    {
                        return RedirectToAction("AdminPanel", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch
                {
                    // If token is invalid, clear it and show register page
                    Response.Cookies.Delete("JwtToken");
                }
            }
            return View();
        }

        // POST: /Auth/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // Validate phone number format
            if (!string.IsNullOrEmpty(request.Phone))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^01[0-9]{9}$"))
                {
                    ModelState.AddModelError("Phone", "Phone number must start with '01' and be exactly 11 digits");
                    return View(request);
                }
            }

            // Check if the user already exists
            if (_context.Users.Any(u => u.Email == request.Email && u.Name == request.Name))
            {
                ModelState.AddModelError("", "User already exists.");
                return View(request);
            }

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                HashedPassword = passwordHash,
                Address = request.Address,
                Phone = request.Phone,
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}

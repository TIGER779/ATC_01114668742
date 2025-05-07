using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AreebTechnologyTask.Dto;
using AreebTechnologyTask.Models;
using AreebTechnologyTask.Data;
using AreebTechnologyTask.Enums;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;


namespace AreebTechnologyTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // Check if user already exists
            if (_context.Users.Any(u => u.Email == request.Email))
                return BadRequest("User already exists.");

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new user object
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                HashedPassword = passwordHash,
                Address = request.Address,
                Phone = request.Phone,
                Role = UserRole.User // Default to "User" role
            };

            // Add user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            Console.WriteLine($"Email from request: {request.Email} ({request.Email.GetType()})");
            var user = await _context.Users.FirstOrDefaultAsync();
            Console.WriteLine($"First user: {user?.Email}");

            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(request.Email));

            if (user != null)
                Console.WriteLine($"User.Email from DB: {user.Email} ({user.Email.GetType()})");


            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            string token = CreateToken(user);
            return Ok(new { token });
        }


        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

